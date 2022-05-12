using System.Collections.Generic;
using System.IO;
using System.Linq;
using Map;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Structures
{
    [RequireComponent(typeof(MeshFilter))]
    public class Section : MonoBehaviour
    {
        private static readonly Vector3[] Corners =
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 1, -0.5f),
        };

        private const float NoiseScale = .5f;
        private const float HeightFactor = 1.5f;
        private static string Directory => Application.dataPath + "/Resources" + "/SectionData/";
        private string FileName => MeshFilter.sharedMesh.name;
        private string FilePath => Directory + FileName;

        public Mesh ruinedModel;
        public bool randomRotations;
        public Vector2 randomScale = Vector2.one;
        public bool hasGrass = true;

        [SerializeField] private List<Mesh> meshVariants; 
        
        private List<Vector3> _cellCorners;
        private ComputeShader _meshCompute;
        private MeshFilter _meshFilter;
        private bool _usesShader;
        private static readonly int HasGrass = Shader.PropertyToID("_HasGrass");
        private static readonly int RoofColor = Shader.PropertyToID("_RoofColor");

        private MeshFilter MeshFilter => _meshFilter ? _meshFilter : _meshFilter = GetComponent<MeshFilter>();

        private void Awake()
        {
            _meshCompute = (ComputeShader)Resources.Load("SectionCompute");
            _usesShader = _meshCompute != null && 
                          (Application.platform == RuntimePlatform.WindowsPlayer || 
                           Application.platform == RuntimePlatform.WindowsEditor);
            GetComponent<Renderer>().material.SetInt(HasGrass, hasGrass ? 1 : 0);
        }

        public void Init(Cell cell, bool fitToCell = false, bool isRuin = false, int clockwiseRotations = 0)
        {
            if (meshVariants.Count > 0)
            {
                Random.InitState(cell.Id);
                MeshFilter.sharedMesh = meshVariants.SelectRandom();
            }
            
            _cellCorners = Manager.Map.GetCornerPositions(cell);

            // Offset corners
            for (int i = 0; i < clockwiseRotations; i++)
            {
                Vector3 temp = _cellCorners[0];
                _cellCorners.RemoveAt(0);
                _cellCorners.Add(temp);
            }

            if (isRuin)
            {
                ToRuin();
            }

            else if (fitToCell)
            {
                Fit();
            }
            else
            {
                Transform t = transform;
                
                t.position = new Vector3 (
                    _cellCorners.Average(x => x.x), 0,
                    _cellCorners.Average(x => x.z)
                );
                
                // Perlin noise for size scale, was breaking so just using normal random
                //float noise = Mathf.PerlinNoise(pos.x * NoiseScale, pos.z * NoiseScale);
                //t.localScale = Vector3.one * Mathf.Lerp(randomScale.x, randomScale.y, noise);
                t.localScale = Vector3.one * Random.Range(randomScale.x, randomScale.y);

                // Either completely random or just rotating horizontally
                if (randomRotations) t.rotation = Random.rotation;
                else t.eulerAngles = new Vector3(0, Random.value * 360, 0);
            }
        }


        private void Fit()
        {
            // Retrieve the section data
            SectionData sectionData = Manager.Structures.BuildingCache[FileName];

            // Calculate new vertex positions
            Vector3[] planePositions = new Vector3[MeshFilter.mesh.vertexCount];

            if (_usesShader)
            {
                ComputeBuffer sectionBuffer = new ComputeBuffer(sectionData.VertexCoordinates.Length, sizeof(float) * 3);
                ComputeBuffer vertexBuffer = new ComputeBuffer(planePositions.Length, sizeof(float) * 3);
                ComputeBuffer cornerBuffer = new ComputeBuffer(_cellCorners.Count, sizeof(float) * 3);
                sectionBuffer.SetData(sectionData.VertexCoordinates);
                vertexBuffer.SetData(planePositions);
                cornerBuffer.SetData(_cellCorners);
            
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
                    Vector3 i0 = Vector3.Lerp(_cellCorners[0], _cellCorners[3], sectionData[i].x);
                    Vector3 i1 = Vector3.Lerp(_cellCorners[1], _cellCorners[2], sectionData[i].x);
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
            GetComponent<MeshRenderer>().material.SetColor(RoofColor, color);
        }

        [Button("To Ruin")]
        public void ToRuin()
        {
            if (ruinedModel == null)
            {
                Debug.LogError("No ruined model for " + name);
                return;
            }
            MeshFilter.mesh = ruinedModel;
            GetComponent<MeshRenderer>().material.SetFloat("_Exponent", 1);
            Fit();
        }

        [Button("Save Deform Coordinates to Text File")]
        public void Save()
        {
            SectionData sectionData = new SectionData(MeshFilter);
            File.WriteAllText(FilePath + ".json", JsonUtility.ToJson(sectionData));

            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        #if UNITY_EDITOR
        public static void EditorSave(MeshFilter mf)
        {
            SectionData sectionData = new SectionData(mf);
            File.WriteAllText(Directory + mf.sharedMesh.name + ".json", JsonUtility.ToJson(sectionData));
            UnityEditor.AssetDatabase.Refresh();
        }
        #endif

        [System.Serializable]
        public class SectionData
        {
            public Vector3[] VertexCoordinates;

            public SectionData(MeshFilter meshFilter)
            {
                VertexCoordinates = Calculate(meshFilter);
            }

            public static Vector3[] Calculate(MeshFilter mf)
            {
                Vector3[] worldVertices = mf.sharedMesh.vertices;
                for (int i = 0; i < worldVertices.Length; i++)
                    worldVertices[i] = mf.transform.TransformPoint(worldVertices[i]);

                int vertexCount = worldVertices.Length;
                Vector3[] uv = new Vector3[vertexCount];

                Vector3 p0 = Corners[0];
                Vector3 p1 = Corners[1];
                Vector3 p2 = Corners[4];
                Vector3 p3 = Corners[3];

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
