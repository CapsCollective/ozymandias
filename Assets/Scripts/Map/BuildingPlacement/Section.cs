using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingPlacement
{
    [RequireComponent(typeof(MeshFilter))]
    public class Section : MonoBehaviour
    {
        // Member variables
        public Transform cornerParent;
        
        private MeshFilter _meshFilter;

        // Properties
        public Vector2[] VertexCoordinates { get; private set; }

        // MonoBehaviour Functions
        private void OnDrawGizmos()
        {
            for (int i = 0; i < 4; i++)
            {
                Gizmos.color = Color.Lerp(Color.blue, Color.red, i / (3.0f));
                Gizmos.DrawSphere(cornerParent.GetChild(i).position, .1f);
            }
        }

        // Class Functions
        public void Fit(Vector3[] corners)
        {
            _meshFilter = GetComponent<MeshFilter>();

            CalculateVertexCoordinates();

            //Debug.Log(corners[0] + " : " + corners[1] + " : " + corners[2] + " : " + corners[3]);

            Vector3[] planePositions = new Vector3[_meshFilter.mesh.vertexCount];

            for (int i = 0; i < planePositions.Length; i++)
            {
                Vector3 i0 = Vector3.Lerp(corners[0], corners[3], VertexCoordinates[i].x);
                Vector3 i1 = Vector3.Lerp(corners[1], corners[2], VertexCoordinates[i].x);
                Vector3 i2 = Vector3.Lerp(i0, i1, VertexCoordinates[i].y);

                planePositions[i] = transform.InverseTransformPoint(i2);
            }

            Vector3[] vertices = _meshFilter.mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Set(planePositions[i].x, vertices[i].y, planePositions[i].z);
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
            VertexCoordinates = new Vector2[vertexCount];

            Vector2 p0 = new Vector2(cornerParent.GetChild(0).position.x, cornerParent.GetChild(0).position.z);
            Vector2 p1 = new Vector2(cornerParent.GetChild(1).position.x, cornerParent.GetChild(1).position.z);
            Vector2 p2 = new Vector2(cornerParent.GetChild(2).position.x, cornerParent.GetChild(2).position.z);
            Vector2 p3 = new Vector2(cornerParent.GetChild(3).position.x, cornerParent.GetChild(3).position.z);

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 vertex = transform.TransformPoint(vertices[i]);
                Vector2 p = new Vector2(vertex.x, vertex.z);
                VertexCoordinates[i] = CalculateUV(p, p0, p1, p2, p3);
            }
        }

        public Vector2 CalculateUV(Vector2 p, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            return new Vector2(InverseLerp(a, d, p), InverseLerp(a, b, p));
        }

        public float InverseLerp(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 AB = b - a;
            Vector2 AV = p - a;
            return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
        }
    }
}
