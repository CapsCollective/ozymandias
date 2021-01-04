using UnityEngine;
using UnityEngine.Rendering;

public class BoidManager : MonoBehaviour {
    private const int ThreadGroupSize = 1024;
    public BoidSettings settings;
    public ComputeShader compute;
    public Transform target;
    
    private Boid[] _boids;
    private bool _usesComputeShader;

    private void Start ()
    {
        // Set boid manager to not use compute shaders for macOS and Linux
        _usesComputeShader = (Application.platform == RuntimePlatform.WindowsEditor || 
                            Application.platform == RuntimePlatform.WindowsPlayer);
        
        _boids = FindObjectsOfType<Boid> ();
        foreach (Boid b in _boids) {
            b.Initialize (settings, target);
        }
    }

    private void Update ()
    {
        if (_boids == null) return;
        int numBoids = _boids.Length;
        var boidData = new BoidData[numBoids];

        for (int i = 0; i < _boids.Length; i++) {
            boidData[i].position = _boids[i].position;
            boidData[i].direction = _boids[i].forward;
        }

        if (_usesComputeShader)
        {
            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", _boids.Length);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(numBoids / (float) ThreadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            AsyncGPUReadback.Request(boidBuffer);
            UpdateBoids(boidData);
                
            boidBuffer.Release();
        }
        else
        {
            boidData = GetData(boidData);
            UpdateBoids(boidData);
        }
    }

    private void UpdateBoids(BoidData[] boidData)
    {
        for (int i = 0; i < _boids.Length; i++) {
            _boids[i].avgFlockHeading = boidData[i].flockHeading;
            _boids[i].centreOfFlockmates = boidData[i].flockCentre;
            _boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
            _boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

            _boids[i].UpdateBoid();
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
