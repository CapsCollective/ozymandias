using System.IO;
using Managers;
using NaughtyAttributes;
using UnityEngine;

namespace Structures
{
    [RequireComponent(typeof(MeshFilter))]
    public class Section : MonoBehaviour
    {
        private const float NoiseScale = .5f;
        private const float HeightFactor = 1.5f;
        private static string Directory => Application.dataPath + "/Resources" + "/SectionData/";
        private string FileName => MeshFilter.sharedMesh.name;
        private string FilePath => Directory + FileName;

        // Member variables
        public Transform cornerParent;
        public bool randomRotations;
        public Vector2 randomScale = Vector2.one;
        public bool hasGrass = true;
        public bool debug;
        private ComputeShader _meshCompute;
        
        private MeshFilter _meshFilter;
        private bool _usesShader;
        private static readonly int HasGrass = Shader.PropertyToID("_HasGrass");
        private static readonly int RoofColor = Shader.PropertyToID("_RoofColor");

        private MeshFilter MeshFilter => _meshFilter ? _meshFilter : _meshFilter = GetComponent<MeshFilter>();

        // MonoBehaviour Functions
        private void OnDrawGizmos()
        {
            if (!debug) return;
            for (int i = 0; i < 5; i++)
            {
                Gizmos.color = Color.Lerp(Color.blue, Color.red, i / (4.0f));
                Gizmos.DrawSphere(cornerParent.GetChild(i).position, .1f);
            }
        }

        private void Awake()
        {
            _meshCompute = (ComputeShader)Resources.Load("SectionCompute");
            _usesShader = _meshCompute != null && 
                          (Application.platform == RuntimePlatform.WindowsPlayer || 
                           Application.platform == RuntimePlatform.WindowsEditor);
        
            if (randomRotations) transform.rotation = Random.rotation;
            Vector3 pos = transform.position;
            float noise = Mathf.PerlinNoise(pos.x * NoiseScale, pos.z * NoiseScale);
            transform.localScale = Vector3.one * Mathf.Lerp(randomScale.x, randomScale.y, noise);

            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetInt(HasGrass, hasGrass ? 1 : 0);
            GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
        }

        // Class Functions
        public void Fit(Vector3[] corners, int clockwiseRotations)
        {
            for (int i = 0; i < clockwiseRotations; i++)
            {
                Vector3 temp = corners[0];
                corners[0] = corners[1];
                corners[1] = corners[2];
                corners[2] = corners[3];
                corners[3] = temp;
            }

            // Retrieve the section data
            SectionData sectionData = GameManager.Manager.Structures.BuildingCache[FileName];

            // Calculate new vertex positions
            Vector3[] planePositions = new Vector3[MeshFilter.mesh.vertexCount];

            if (_usesShader)
            {
                ComputeBuffer sectionBuffer = new ComputeBuffer(sectionData.VertexCoordinates.Length, sizeof(float) * 3);
                ComputeBuffer vertexBuffer = new ComputeBuffer(planePositions.Length, sizeof(float) * 3);
                ComputeBuffer cornerBuffer = new ComputeBuffer(corners.Length, sizeof(float) * 3);
                sectionBuffer.SetData(sectionData.VertexCoordinates);
                vertexBuffer.SetData(planePositions);
                cornerBuffer.SetData(corners);
            
                _meshCompute.SetBuffer(0, "sectionBuffer", sectionBuffer);
                _meshCompute.SetBuffer(0, "vertexBuffer", vertexBuffer);
                _meshCompute.SetBuffer(0, "cornerBuffer", cornerBuffer);
                _meshCompute.SetFloat("heightFactor", HeightFactor);
                _meshCompute.SetInt("vertexCount", sectionData.VertexCoordinates.Length);

                if(planePositions.Length / 512 > 0)
                    _meshCompute.Dispatch(0, planePositions.Length / 512, 8, 1);
                else
                    _meshCompute.Dispatch(0, 1, 8, 1);
            
                vertexBuffer.GetData(planePositions);

                sectionBuffer.Release();
                vertexBuffer.Release();
                cornerBuffer.Release();
            }

            for (int i = 0; i < planePositions.Length; i++)
            {
                Vector3 CalculateMesh()
                {
                    Vector3 i0 = Vector3.Lerp(corners[0], corners[3], sectionData[i].x);
                    Vector3 i1 = Vector3.Lerp(corners[1], corners[2], sectionData[i].x);
                    return Vector3.Lerp(i0, i1, sectionData[i].z);
                }

                planePositions[i] = transform.InverseTransformPoint(
                    _usesShader ? planePositions[i] : CalculateMesh());
                planePositions[i].y += HeightFactor * sectionData[i].y;
            }
            // Apply morphed vertices to Mesh Filter
            MeshFilter.mesh.vertices = planePositions;

            MeshFilter.mesh.RecalculateNormals();
            MeshFilter.mesh.RecalculateBounds();
            MeshFilter.mesh.RecalculateTangents();
        
            MeshCollider mc = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = MeshFilter.sharedMesh;
            mc.convex = true;
        }

        public void SetRoofColor(Color color)
        {
            //MaterialPropertyBlock props = new MaterialPropertyBlock();
            //props.SetColor("_RoofColor", color);

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            //renderer.SetPropertyBlock(props);
            meshRenderer.material.SetColor(RoofColor, color);
        }

        [Button("Save Deform Coordinates to Text File")]
        public void Save()
        {
            SectionData sectionData = new SectionData(MeshFilter, cornerParent);
            File.WriteAllText(FilePath + ".json", JsonUtility.ToJson(sectionData));

            #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
            #endif
        }
    
        [System.Serializable]
        public class SectionData
        {
            public Vector3[] VertexCoordinates;

            public SectionData(MeshFilter meshFilter, Transform cornerParent)
            {
                VertexCoordinates = Calculate(meshFilter, cornerParent);
            }

            public static Vector3[] Calculate(MeshFilter mf, Transform cornerParent)
            {
                Vector3[] worldVertices = mf.sharedMesh.vertices;
                for (int i = 0; i < worldVertices.Length; i++)
                    worldVertices[i] = mf.transform.TransformPoint(worldVertices[i]);

                int vertexCount = worldVertices.Length;
                Vector3[] uv = new Vector3[vertexCount];

                Vector3 p0 = cornerParent.GetChild(0).position;
                Vector3 p1 = cornerParent.GetChild(1).position;
                Vector3 p2 = cornerParent.GetChild(4).position;
                Vector3 p3 = cornerParent.GetChild(3).position;

                for (int i = 0; i < vertexCount; i++)
                {
                    Vector3 p = worldVertices[i];
                    uv[i] = CalculateUV(p, p0, p1, p2, p3);
                }

                return uv;
            }

            private static Vector3 CalculateUV(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            {
                return new Vector3(InverseLerp(a, d, p), InverseLerp(a, c, p), InverseLerp(a, b, p));
            }

            private static float InverseLerp(Vector3 a, Vector3 b, Vector3 p)
            {
                Vector3 ab = b - a;
                Vector3 ap = p - a;
                return Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab);
            }

            public Vector3 this[int index] => VertexCoordinates[index];
        }
    }
}
