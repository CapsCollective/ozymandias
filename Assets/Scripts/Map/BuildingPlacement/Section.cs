using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Section : MonoBehaviour
{
    // Member variables
    public int clockwiseRotations;
    public Transform cornerParent;

    private MeshFilter _meshFilter;

    // Properties
    public Vector3[] VertexCoordinates { get; private set; }

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
        // Rotate the mesh
        for (int i = 0; i < clockwiseRotations; i++)
        {
            Vector3 temp = corners[0];
            corners[0] = corners[3];
            corners[3] = corners[2];
            corners[2] = corners[1];
            corners[1] = temp;
        }

        _meshFilter = GetComponent<MeshFilter>();

        CalculateVertexCoordinates();

        //Debug.Log(corners[0] + " : " + corners[1] + " : " + corners[2] + " : " + corners[3]);

        Vector3[] planePositions = new Vector3[_meshFilter.mesh.vertexCount];

        for (int i = 0; i < planePositions.Length; i++)
        {
            Vector3 i0 = Vector3.Lerp(corners[0], corners[3], VertexCoordinates[i].x);
            Vector3 i1 = Vector3.Lerp(corners[1], corners[2], VertexCoordinates[i].x);
            Vector3 i2 = Vector3.Lerp(i0, i1, VertexCoordinates[i].z);

            planePositions[i] = transform.InverseTransformPoint(i2);
            planePositions[i].y += heightFactor * VertexCoordinates[i].y;
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

    public void CalculateVertexCoordinates()
    {
        int vertexCount = _meshFilter.mesh.vertexCount;
        Vector3[] vertices = _meshFilter.mesh.vertices;
        VertexCoordinates = new Vector3[vertexCount];

        Vector3 p0 = cornerParent.GetChild(0).position;
        Vector3 p1 = cornerParent.GetChild(1).position;
        Vector3 p2 = cornerParent.GetChild(4).position;
        Vector3 p3 = cornerParent.GetChild(3).position;

        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 p = transform.TransformPoint(vertices[i]);
            VertexCoordinates[i] = CalculateUV(p, p0, p1, p2, p3);
        }
    }

    public Vector3 CalculateUV(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        return new Vector3(InverseLerp(a, d, p), InverseLerp(a, c, p), InverseLerp(a, b, p));
    }

    public float InverseLerp(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 AB = b - a;
        Vector3 AP = p - a;
        return Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB);
    }
}