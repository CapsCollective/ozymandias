using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Section : MonoBehaviour
{
    private const float NoiseScale = 5f;

    // Member variables
    public int clockwiseRotations;
    public Transform cornerParent;
    public bool randomRotations;
    public Vector2 randomScale = Vector2.one;
    public bool hasGrass = true;
    public bool debug;

    private MeshFilter _meshFilter;
    private bool _usesShader;
    [SerializeField] private ComputeShader _meshCompute;
    private static readonly int HasGrass = Shader.PropertyToID("_HasGrass");
    private static readonly int RoofColor = Shader.PropertyToID("_RoofColor");

    // Properties
    private string Directory => Application.dataPath + "/Resources" + "/SectionData/";
    private string FileName => MeshFilter.sharedMesh.name;
    private string FilePath => Directory + FileName;

    private MeshFilter MeshFilter => _meshFilter ? _meshFilter : _meshFilter = GetComponent<MeshFilter>();

    // MonoBehaviour Functions
    private void OnDrawGizmos()
    {
        if (!debug) return;
        for (var i = 0; i < 5; i++)
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
        
        if (randomRotations)
            transform.rotation = Random.rotation;
        var pos = transform.position;
        var noise = Mathf.PerlinNoise(pos.x * NoiseScale, pos.z * NoiseScale);
        transform.localScale = Vector3.one * Mathf.Lerp(randomScale.x, randomScale.y, noise);

        var materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetInt(HasGrass, hasGrass ? 1 : 0);
        GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
    }

    // Class Functions
    public void Fit(Vector3[] corners, float heightFactor)
    {
        for (var i = 0; i < clockwiseRotations; i++)
        {
            var temp = corners[0];
            corners[0] = corners[1];
            corners[1] = corners[2];
            corners[2] = corners[3];
            corners[3] = temp;
        }

        // Retrieve the section data
        var sectionData = Managers.GameManager.Manager.Buildings._buildingCache[FileName];

        // Calculate new vertex positions
        var planePositions = new Vector3[MeshFilter.mesh.vertexCount];

        if (_usesShader)
        {
            var sectionBuffer = new ComputeBuffer(sectionData.VertexCoordinates.Length, sizeof(float) * 3);
            var vertexBuffer = new ComputeBuffer(planePositions.Length, sizeof(float) * 3);
            var cornerBuffer = new ComputeBuffer(corners.Length, sizeof(float) * 3);
            sectionBuffer.SetData(sectionData.VertexCoordinates);
            vertexBuffer.SetData(planePositions);
            cornerBuffer.SetData(corners);
            
            _meshCompute.SetBuffer(0, "sectionBuffer", sectionBuffer);
            _meshCompute.SetBuffer(0, "vertexBuffer", vertexBuffer);
            _meshCompute.SetBuffer(0, "cornerBuffer", cornerBuffer);
            _meshCompute.SetFloat("heightFactor", heightFactor);
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

        for (var i = 0; i < planePositions.Length; i++)
        {
            Vector3 CalculateMesh()
            {
                var i0 = Vector3.Lerp(corners[0], corners[3], sectionData[i].x);
                var i1 = Vector3.Lerp(corners[1], corners[2], sectionData[i].x);
                return Vector3.Lerp(i0, i1, sectionData[i].z);
            }

            planePositions[i] = transform.InverseTransformPoint(
                _usesShader ? planePositions[i] : CalculateMesh());
            planePositions[i].y += heightFactor * sectionData[i].y;
        }
        // Apply morphed vertices to Mesh Filter
        MeshFilter.mesh.vertices = planePositions;

        MeshFilter.mesh.RecalculateNormals();
        MeshFilter.mesh.RecalculateBounds();
        MeshFilter.mesh.RecalculateTangents();
        
        var mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = MeshFilter.sharedMesh;
        mc.convex = true;
    }

    public SectionData GetSectionData()
    {
        if (!File.Exists(FilePath))
            Save();

        return Load();
    }

    public void Save()
    {
        var sectionData = new SectionData(MeshFilter, cornerParent);
        File.WriteAllText(FilePath + ".json", JsonUtility.ToJson(sectionData));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private SectionData Load()
    {
        return Managers.GameManager.Manager.Buildings._buildingCache[FileName];
    }

    public void SetRoofColor(Color color)
    {
        //MaterialPropertyBlock props = new MaterialPropertyBlock();
        //props.SetColor("_RoofColor", color);

        var meshRenderer = GetComponent<MeshRenderer>();
        //renderer.SetPropertyBlock(props);
        meshRenderer.material.SetColor(RoofColor, color);
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
            var worldVertices = mf.sharedMesh.vertices;
            for (var i = 0; i < worldVertices.Length; i++)
                worldVertices[i] = mf.transform.TransformPoint(worldVertices[i]);

            var vertexCount = worldVertices.Length;
            var vertices = worldVertices;
            var uv = new Vector3[vertexCount];

            var p0 = cornerParent.GetChild(0).position;
            var p1 = cornerParent.GetChild(1).position;
            var p2 = cornerParent.GetChild(4).position;
            var p3 = cornerParent.GetChild(3).position;

            for (var i = 0; i < vertexCount; i++)
            {
                var p = vertices[i];
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
            var ab = b - a;
            var ap = p - a;
            return Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab);
        }

        public Vector3 this[int index] => VertexCoordinates[index];
    }
}