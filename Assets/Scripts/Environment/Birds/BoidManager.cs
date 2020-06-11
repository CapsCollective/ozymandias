using UnityEngine;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;
    public BoidSettings settings;
    public ComputeShader compute;
    public Transform target;
    Boid[] boids;
    
    private bool usesComputeShader;

    void Start ()
    {
        // Set boid manager to not use compute shaders for macOS and Linux
        usesComputeShader = (Application.platform == RuntimePlatform.WindowsEditor || 
                             Application.platform == RuntimePlatform.WindowsPlayer);
        
        boids = FindObjectsOfType<Boid> ();
        foreach (Boid b in boids) {
            b.Initialize (settings, target);
        }

    }

    void Update () {
        if (boids != null) {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            if (usesComputeShader)
            {
                var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
                boidBuffer.SetData(boidData);

                compute.SetBuffer(0, "boids", boidBuffer);
                compute.SetInt("numBoids", boids.Length);
                compute.SetFloat("viewRadius", settings.perceptionRadius);
                compute.SetFloat("avoidRadius", settings.avoidanceRadius);

                int threadGroups = Mathf.CeilToInt(numBoids / (float) threadGroupSize);
                compute.Dispatch(0, threadGroups, 1, 1);

                boidBuffer.GetData(boidData);

                UpdateBoids(boidData);
                
                boidBuffer.Release();
            }
            else
            {
                boidData = GetData(boidData);
                UpdateBoids(boidData);
            }
        }
    }

    private void UpdateBoids(BoidData[] boidData)
    {
        for (int i = 0; i < boids.Length; i++) {
            boids[i].avgFlockHeading = boidData[i].flockHeading;
            boids[i].centreOfFlockmates = boidData[i].flockCentre;
            boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
            boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

            boids[i].UpdateBoid();
        }
    }

    // Copy of compute shader logic without GPU threading
    private BoidData[] GetData(BoidData[] boids)
    {
        int numBoids = boids.Length;
        float viewRadius = settings.perceptionRadius;
        float avoidRadius = settings.avoidanceRadius;
        
        for (int i = 0; i < numBoids; i++) {
            Vector3 offset = boids[i].position - boids[i].position;
            float sqrDst = offset.x * offset.x + offset.y * offset.y + offset.z * offset.z;

            if (sqrDst < viewRadius * viewRadius) {
                boids[i].numFlockmates += 1;
                boids[i].flockHeading += boids[i].direction;
                boids[i].flockCentre += boids[i].position;

                if (sqrDst < avoidRadius * avoidRadius) {
                    boids[i].avoidanceHeading -= offset / sqrDst;
                }
            }
        }
        
        return boids;
    }

    public struct BoidData {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}