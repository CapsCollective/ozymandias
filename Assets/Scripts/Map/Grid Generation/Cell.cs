using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private BuildingMesh occupant;

    public List<Vertex> Vertices { get; private set; }
    public Vector3 Centre { get { return (Vertices[0] + Vertices[1] + Vertices[2] + Vertices[3]) / 4; } }
    public bool Occupied { get { return occupant; } }

    public Cell(Triangle triA, Triangle triB)
    {
        Vertices = new List<Vertex>(triA.Vertices);

        // STEP 1. Find the unshared vertex in triB
        Vertex unsharedB = triB.Vertices[0];
        for (int i = 0; i < triB.Vertices.Count; i++)
        {
            if (!triA.Vertices.Contains(triB.Vertices[i]))
                unsharedB = triB.Vertices[i];
        }

        // STEP 2. Loop through Vertices and find the first vertex where i - 1 is not contained by triB
        int splitStartIndex = 0;
        for (int i = 0; i < triA.Vertices.Count; i++)
        {
            if (!triB.Vertices.Contains(triA.Vertices[(i - 1 + 3) % 3]))
                splitStartIndex = i;
        }

        // STEP 3. Add the unshared vertex from triB between the found vertex i and i + 1
        Vertices.Insert(splitStartIndex + 1 % 3, unsharedB);
    }

    public Cell(Vertex vertexA, Vertex vertexB, Vertex vertexC, Vertex vertexD)
    {
        Vertices = new List<Vertex> { vertexA, vertexB, vertexC, vertexD };
    }

    public void Occupy(BuildingMesh newOccupant)
    {
        occupant = newOccupant;
    }

    public Cell[] Subdivide()
    {
        Cell[] newCells = new Cell[4];
        List<Vertex> subVertices = new List<Vertex>();

        // STEP 1. Add the split vertices between the existing vertices
        for (int i = 0; i < Vertices.Count; i++)
        {
            subVertices.Add(Vertices[i]);
            subVertices.Add(
                new Vertex(
                    (Vertices[i] + Vertices[(i + 1) % Vertices.Count]) / 2
                    )
                );
        }

        // STEP 2. Create the central vertex
        Vertex centralVertex = new Vertex((subVertices[0] + subVertices[2] + subVertices[4] + subVertices[6]) / 4);

        // STEP 3. Iterate through the border vertices and create cells
        for (int i = 0; i < 4; i++)
        {
            newCells[i] = new Cell(
                subVertices[i * 2],
                subVertices[(i * 2 + 9) % 8],
                centralVertex,
                subVertices[(i * 2 + 7) % 8]
                );
        }

        return newCells;
    }

    public void ReplaceVertex(Vertex oldVert, Vertex newVert)
    {
        int index = Vertices.IndexOf(oldVert);
        Vertices[index] = newVert;
    }

    public Vertex[] GetAdjacent(Vertex root)
    {
        int index = Vertices.IndexOf(root);
        return new Vertex[] { Vertices[(index - 1 + Vertices.Count) % Vertices.Count], Vertices[(index + 1 + Vertices.Count) % Vertices.Count] };
    }

    public void DrawCell()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Gizmos.DrawLine(Vertices[i], Vertices[(i + 1) % Vertices.Count]);
        }
    }

    public static bool operator ==(Cell cell, Cell other)
    {
        if (ReferenceEquals(other, null))
        {
            return ReferenceEquals(cell, null);
        }

        foreach (Vertex vertex in cell.Vertices)
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

    public static bool operator !=(Cell cell, Cell other)
    {
        return !(cell == other);
    }

    public static implicit operator List<Vertex>(Cell cell)
    {
        return cell.Vertices;
    }

    public static implicit operator Vertex[](Cell cell)
    {
        return cell.Vertices.ToArray();
    }

    public override bool Equals(object obj)
    {
        return this == (Cell)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}