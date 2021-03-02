using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MeshFilter))]
public class Section : MonoBehaviour
{
    // Member variables
    public int clockwiseRotations;
    public Transform cornerParent;

    public bool debug;

    private MeshFilter _meshFilter;

    // Properties
    private string FileName { get { return MeshFilter.sharedMesh.name + ".json"; } }
    private string Directory { get { return Application.streamingAssetsPath + "/Section Data/"; } }
    public string FilePath { get { return Directory + FileName; } }

    private MeshFilter MeshFilter
    {
        get { return _meshFilter ? _meshFilter : _meshFilter = GetComponent<MeshFilter>(); }
    }

    // MonoBehaviour Functions
    private void OnDrawGizmos()
    {
        if (debug)
        {
            for (int i = 0; i < 5; i++)
            {
                Gizmos.color = Color.Lerp(Color.blue, Color.red, i / (4.0f));
                Gizmos.DrawSphere(cornerParent.GetChild(i).position, .1f);
            }
        }
    }

    // Class Functions
    public void Fit(Vector3[] corners, float heightFactor)
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
        SectionData sectionData = GetSectionData();

        // Calculate new vertex positions
        Vector3[] planePositions = new Vector3[MeshFilter.mesh.vertexCount];
        for (int i = 0; i < planePositions.Length; i++)
        {
            Vector3 i0 = Vector3.Lerp(corners[0], corners[3], sectionData[i].x);
            Vector3 i1 = Vector3.Lerp(corners[1], corners[2], sectionData[i].x);
            Vector3 i2 = Vector3.Lerp(i0, i1, sectionData[i].z);

            planePositions[i] = transform.InverseTransformPoint(i2);
            planePositions[i].y += heightFactor * sectionData[i].y;
        }

        // Apply morphed vertices to Mesh Filter
        MeshFilter.mesh.vertices = planePositions;

        MeshFilter.mesh.RecalculateNormals();
        MeshFilter.mesh.RecalculateBounds();
        MeshFilter.mesh.RecalculateTangents();
    }

    public SectionData GetSectionData()
    {
        if (!File.Exists(FilePath))
            Save();

        return Load();
    }

    public void Save()
    {
        SectionData sectionData = new SectionData(MeshFilter, cornerParent);
        File.WriteAllText(FilePath, JsonUtility.ToJson(sectionData));

        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    private SectionData Load()
    {
        return JsonUtility.FromJson<SectionData>(File.ReadAllText(FilePath));
    }

    public void SetRoofColor(Color color)
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_RoofColor", color);

        var renderer = GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);
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
            Vector3[] vertices = worldVertices;
            Vector3[] uv = new Vector3[vertexCount];

            Vector3 p0 = cornerParent.GetChild(0).position;
            Vector3 p1 = cornerParent.GetChild(1).position;
            Vector3 p2 = cornerParent.GetChild(4).position;
            Vector3 p3 = cornerParent.GetChild(3).position;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 p = vertices[i];
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
            Vector3 AB = b - a;
            Vector3 AP = p - a;
            return Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB);
        }

        public Vector3 this[int index]
        {
            get => VertexCoordinates[index];
        }
    }
}