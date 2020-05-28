using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public List<Vertex> Vertices { get; private set; }

    public Triangle(Vertex vertexA, Vertex vertexB, Vertex vertexC)
    {
        Vector3 ab = (vertexB - vertexA).normalized;
        Vector3 ac = (vertexC - vertexA).normalized;

        Vertices = Vector3.SignedAngle(ab, ac, Vector3.forward) < 0 ?
            new List<Vertex> { vertexA, vertexB, vertexC } :
            new List<Vertex> { vertexA, vertexC, vertexB };
    }

    public Cell[] Subdivide()
    {
        Cell[] newCells = new Cell[3];
        List<Vertex> subVertices = new List<Vertex>();

        // STEP 1. Add the split vertices between the existing vertices
        for (int i = 0; i < Vertices.Count; i++)
        {
            subVertices.Add(Vertices[i]);
            subVertices.Add(
                new Vertex(
                    (Vertices[i] + Vertices[(i + 1) % Vertices.Count]) / 2,
                    false,
                    false
                    )
                );
        }

        // STEP 2. Create the central vertex
        Vertex centralVertex = new Vertex((subVertices[0] + subVertices[2] + subVertices[4]) / 3, false, false);

        // STEP 3. Iterate through the border vertices and create cells
        for (int i = 0; i < 3; i++)
        {
            newCells[i] = new Cell(
                subVertices[i * 2],
                subVertices[(i * 2 + 7) % 6],
                centralVertex,
                subVertices[(i * 2 + 5) % 6]
                );
        }

        return newCells;
    }

    public static bool operator ==(Triangle triangle, Triangle other)
    {
        foreach (Vertex vertex in triangle.Vertices)
        {
            bool contains = false;

            foreach (Vertex otherVertex in other.Vertices)
            {
                if (vertex == otherVertex)
                    contains = true;
            }

            if (!contains) return false;
        }
        return true;
    }

    public static bool operator !=(Triangle triangle, Triangle other)
    {
        return !(triangle == other);
    }

    public void DrawCell()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Gizmos.DrawLine(Vertices[i], Vertices[(i + 1) % Vertices.Count]);
        }
    }

    public override bool Equals(object obj)
    {
        return this == (Triangle)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}