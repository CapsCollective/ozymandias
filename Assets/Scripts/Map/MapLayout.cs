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
        // STEP 1. Add all triangular cells

        // STEP 2. Remove all possible edges

        // STEP 3. Subdivide

        // STEP 4. Relax
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
        return VertexMap.ContainsKey(root) ? VertexMap[root] : new List<Vertex>();
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
    public Vertex[] Vertices { get; private set; }

    public bool IsQuad { get { return Vertices.Length == 4; } }

    public Cell(Vertex vertexA, Vertex vertexB, Vertex vertexC, Vertex vertexD)
    {
        Vertices = new Vertex[] { vertexA, vertexB, vertexC, vertexD };
    }

    public Cell(Vertex vertexA, Vertex vertexB, Vertex vertexC)
    {
        Vertices = new Vertex[] { vertexA, vertexB, vertexC };
    }

    public static implicit operator Vertex[](Cell cell)
    {
        return cell.Vertices;
    }
}
