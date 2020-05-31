using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Section : MonoBehaviour
{
    // Member variables
    public Transform cornerParent;

    private MeshFilter _meshFilter;

    // Properties
    private SectionData sectionData;

    // MonoBehaviour Functions
    private void OnDrawGizmos()
    {
        for (int i = 0; i < 5; i++)
        {
            Gizmos.color = Color.Lerp(Color.blue, Color.red, i / (4.0f));
            Gizmos.DrawSphere(cornerParent.GetChild(i).position, .1f);
        }
    }

    // Class Functions
    public void Fit(Vector3[] corners, float heightFactor)
    {
        _meshFilter = GetComponent<MeshFilter>();
        Vector3[] worldVertices = _meshFilter.sharedMesh.vertices;

        for (int i = 0; i < worldVertices.Length; i++)
            worldVertices[i] = transform.TransformPoint(worldVertices[i]);

        sectionData = new SectionData();
        sectionData.Calculate(worldVertices, cornerParent);

        Vector3[] planePositions = new Vector3[_meshFilter.mesh.vertexCount];

        for (int i = 0; i < planePositions.Length; i++)
        {
            Vector3 i0 = Vector3.Lerp(corners[0], corners[3], sectionData[i].x);
            Vector3 i1 = Vector3.Lerp(corners[1], corners[2], sectionData[i].x);
            Vector3 i2 = Vector3.Lerp(i0, i1, sectionData[i].z);

            planePositions[i] = transform.InverseTransformPoint(i2);
            planePositions[i].y += heightFactor * sectionData[i].y;
        }

        Vector3[] vertices = _meshFilter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = planePositions[i];
        }

        _meshFilter.mesh.vertices = vertices;
        _meshFilter.mesh.RecalculateNormals();
        _meshFilter.mesh.RecalculateBounds();
        _meshFilter.mesh.RecalculateTangents();
    }

    [System.Serializable]
    public class SectionData
    {
        private Vector3[] VertexCoordinates;

        public void Calculate(Vector3[] worldVertices, Transform cornerParent)
        {
            int vertexCount = worldVertices.Length;
            Vector3[] vertices = worldVertices;
            VertexCoordinates = new Vector3[vertexCount];

            Vector3 p0 = cornerParent.GetChild(0).position;
            Vector3 p1 = cornerParent.GetChild(1).position;
            Vector3 p2 = cornerParent.GetChild(4).position;
            Vector3 p3 = cornerParent.GetChild(3).position;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 p = vertices[i];
                VertexCoordinates[i] = CalculateUV(p, p0, p1, p2, p3);
            }
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

        public static implicit operator Vector3[](SectionData data)
        {
            return data.VertexCoordinates;
        }

        public Vector3 this[int index]
        {
            get => VertexCoordinates[index];
            set => VertexCoordinates[index] = value;
        }
    }
}