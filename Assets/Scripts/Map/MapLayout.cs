using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Layout", menuName = "Map Layout", order = 50)]
public class MapLayout : ScriptableObject
{
    public int depth;

    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();
    public List<Cell> Cells { get; private set; } = new List<Cell>();

    private readonly Dictionary<Vertex, List<Vertex>> VertexMap = new Dictionary<Vertex, List<Vertex>>();
    private readonly Dictionary<Cell, List<Cell>> CellMap = new Dictionary<Cell, List<Cell>>();

    public void Generate()
    {
        GenerateVertices();
        GenerateCells();
    }

    public void GenerateVertices()
    {
        Vertices = new List<Vertex>();

        int minCol = -depth;
        int maxCol = depth;

        float xStep = Mathf.Cos(Mathf.PI / 6f) / depth;
        float yStep = 1f / depth;

        for (int itCol = minCol; itCol <= maxCol; itCol++)
        {
            int verticesInColumn = (depth * 2 + 1) - Mathf.Abs(itCol);
            float x = xStep * itCol;

            for (int itRow = 0; itRow < verticesInColumn; itRow++)
            {
                float y = yStep * (itRow - ((verticesInColumn - 1) / 2f));
                Vertex newVertex = new Vertex(new Vector3(x, y, 0));

                Vertices.Add(newVertex);
                VertexMap.Add(newVertex, new List<Vertex>());
            }
        }

        minCol = -depth;
        maxCol = depth;

        int vertexIndex = 0;

        for (int itCol = minCol; itCol <= maxCol; itCol++)
        {
            int verticesInColumn = (depth * 2 + 1) - Mathf.Abs(itCol);

            for (int itRow = 0; itRow < verticesInColumn; itRow++)
            {
                if (itRow != verticesInColumn - 1)
                {
                    CreateEdge(Vertices[vertexIndex], Vertices[vertexIndex + 1]);
                }
                if (itCol < 0)
                {
                    CreateEdge(Vertices[vertexIndex], Vertices[vertexIndex + verticesInColumn + 1]);
                    CreateEdge(Vertices[vertexIndex], Vertices[vertexIndex + verticesInColumn]);
                }
                if (itCol > 0)
                {
                    CreateEdge(Vertices[vertexIndex], Vertices[vertexIndex - verticesInColumn]);
                    CreateEdge(Vertices[vertexIndex], Vertices[vertexIndex - verticesInColumn - 1]);
                }

                vertexIndex++;
            }
        }
    }

    public void GenerateCells()
    {
        Cells = new List<Cell>();

        // STEP 1. Add all triangular cells
        foreach (Vertex root in Vertices)
        {
            foreach (Vertex neighbour in VertexMap[root])
            {
                foreach (Vertex mutual in VertexMap[neighbour])
                {
                    if (IsAdjacent(mutual, root))
                    {
                        Cell newCell = new Cell(root, neighbour, mutual);
                        if (!Cells.Contains(newCell))
                        {
                            Cells.Add(newCell);
                            CellMap.Add(newCell, new List<Cell>());
                        }
                    }
                }
            }
        }

        // STEP 2. Establish adjacency between triangular cells
        foreach (Cell cell in Cells)
        {
            foreach (Cell other in Cells)
            {
                int shareCount = 0;
                foreach (Vertex vertex in cell.Vertices)
                    if (other.Vertices.Contains(vertex))
                        shareCount++;
                if (shareCount >= 2)
                    CellMap[cell].Add(other);
            }
        }

        // STEP 3. Remove all possible edges

        // STEP 4. Subdivide

        // STEP 5. Relax
    }

    public void CreateDirectedEdge(Vertex from, Vertex to)
    {
        if (VertexMap.ContainsKey(from) && !VertexMap[from].Contains(to))
            VertexMap[from].Add(to);
    }

    public void DestroyDirectedEdge(Vertex from, Vertex to)
    {
        if (VertexMap.ContainsKey(from) && VertexMap[from].Contains(to))
            VertexMap[from].Remove(to);
    }

    public void CreateEdge(Vertex v1, Vertex v2)
    {
        CreateDirectedEdge(v1, v2);
        CreateDirectedEdge(v2, v1);
    }

    public void DestroyEdge(Vertex v1, Vertex v2)
    {
        DestroyDirectedEdge(v1, v2);
        DestroyDirectedEdge(v2, v1);
    }

    public List<Vertex> GetAdjacent(Vertex root)
    {
        if (VertexMap.ContainsKey(root))
            return VertexMap[root];
        else
            throw new System.Exception("Vertex not contained in Vertex adjacency map.");
    }

    public List<Cell> GetAdjacent(Cell root)
    {
        if (CellMap.ContainsKey(root))
            return CellMap[root];
        else
            throw new System.Exception("Cell not contained in Cell adjacency map.");
    }

    public bool IsAdjacent(Vertex root, Vertex other)
    {
        return VertexMap.ContainsKey(root) && VertexMap[root].Contains(other);
    }
}

public class Vertex
{
    public Vector3 position;

    public Vertex(Vector3 _position)
    {
        position = _position;
    }

    public static implicit operator Vector3(Vertex vertex)
    {
        return vertex.position;
    }
}

public class Cell
{
    public List<Vertex> Vertices { get; private set; }

    public bool IsQuad { get { return Vertices.Count == 4; } }

    public Cell(Vertex vertexA, Vertex vertexB, Vertex vertexC, Vertex vertexD)
    {
        Vertices = new List<Vertex> { vertexA, vertexB, vertexC, vertexD };
    }

    public Cell(Vertex vertexA, Vertex vertexB, Vertex vertexC)
    {
        Vertices = new List<Vertex> { vertexA, vertexB, vertexC };
    }

    public void DrawCellGizmo()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Gizmos.DrawLine(Vertices[i], Vertices[(i + 1) % Vertices.Count]);
        }
    }

    public static bool operator ==(Cell cell, Cell other)
    {
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
