using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Layout", menuName = "Map Layout", order = 50)]
public class MapLayout : ScriptableObject
{
    public int depth;
    public int seed;

    public int relaxIterations;
    [Range(0f, 1f)] public float relaxStrength;

    public Graph<Vertex> VertexGraph { get; private set; } = new Graph<Vertex>();
    public Graph<Triangle> TriangleGraph { get; private set; } = new Graph<Triangle>();
    public Graph<Cell> CellGraph { get; private set; } = new Graph<Cell>();

    public void Occupy(Vector3 unitPos)
    {
        // Iterate through all of the cells
        // Find the cell that is closest to unitPos
        // Set it to occupied

        Cell closest = CellGraph.GetData()[0];
        float minDist = Vector3.Distance(closest.Centre, unitPos);
        foreach (Cell cell in CellGraph.GetData())
        {
            float distance;
            if ((distance = Vector3.Distance(unitPos, cell.Centre)) < minDist)
            {
                minDist = distance;
                closest = cell;
            }
        }

        closest.occupied = true;
    }

    public void Generate(int seed)
    {
        GenerateVertices();
        GenerateCells(seed);
    }

    public void GenerateVertices()
    {
        VertexGraph = new Graph<Vertex>();

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

                VertexGraph.Add(newVertex);
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
                    VertexGraph.CreateEdge(vertexIndex, vertexIndex + 1);
                }
                if (itCol < 0)
                {
                    VertexGraph.CreateEdge(vertexIndex, vertexIndex + verticesInColumn + 1);
                    VertexGraph.CreateEdge(vertexIndex, vertexIndex + verticesInColumn);
                }
                if (itCol > 0)
                {
                    VertexGraph.CreateEdge(vertexIndex, vertexIndex - verticesInColumn);
                    VertexGraph.CreateEdge(vertexIndex, vertexIndex - verticesInColumn - 1);
                }

                vertexIndex++;
            }
        }
    }

    public void GenerateCells(int seed)
    {
        Random.InitState(seed);

        TriangleGraph = new Graph<Triangle>();

        // STEP 1. Add all triangles
        foreach (Vertex root in VertexGraph.GetData())
        {
            foreach (Vertex neighbour in VertexGraph.GetAdjacent(root))
            {
                foreach (Vertex mutual in VertexGraph.GetAdjacent(neighbour))
                {
                    if (VertexGraph.IsAdjacent(mutual, root))
                    {
                        Triangle newTriangle = new Triangle(root, neighbour, mutual);
                        if (!TriangleGraph.Contains(newTriangle))
                        {
                            TriangleGraph.Add(newTriangle);
                        }
                    }
                }
            }
        }

        // STEP 2. Establish adjacency between triangles
        foreach (Triangle triangle in TriangleGraph.GetData())
        {
            foreach (Triangle other in TriangleGraph.GetData())
            {
                int shareCount = 0;
                foreach (Vertex vertex in triangle.Vertices)
                    if (other.Vertices.Contains(vertex))
                        shareCount++;
                if (shareCount == 2)
                    TriangleGraph.CreateEdge(triangle, other);
            }
        }

        // STEP 3. Merge triangles into cells
        CellGraph = new Graph<Cell>();
        Graph<Triangle> dupTriangleGraph = new Graph<Triangle>();
        while (TriangleGraph.Count > 0)
        {
            Triangle randomRoot = TriangleGraph.GetData()[Random.Range(0, TriangleGraph.Count)];
            if (TriangleGraph.GetAdjacent(randomRoot).Count > 0)
            {
                List<Triangle> adjacent = TriangleGraph.GetAdjacent(randomRoot);
                Triangle randomNeighbour = adjacent[Random.Range(0, adjacent.Count)];

                TriangleGraph.Remove(randomRoot);
                TriangleGraph.Remove(randomNeighbour);

                // Add new Cell combining randomRoot and randomNeighbour to CellGraph
                CellGraph.Add(new Cell(randomRoot, randomNeighbour));
            }
            else
            {
                TriangleGraph.Remove(randomRoot);
                dupTriangleGraph.Add(randomRoot);
            }
        }

        // STEP 5. Subdivide Triangles
        for (int i = CellGraph.Count - 1; i >= 0; i--)
        {
            foreach (Cell newCell in CellGraph.GetData()[i].Subdivide())
                CellGraph.Add(newCell);

            CellGraph.RemoveAt(i);
        }

        // STEP 4. Subdivide Cells
        for (int i = dupTriangleGraph.Count - 1; i >= 0; i--)
        {
            foreach (Cell newCell in dupTriangleGraph.GetData()[i].Subdivide())
                CellGraph.Add(newCell);

            dupTriangleGraph.RemoveAt(i);
        }

        // STEP 6. Recreate vertex graph and use for cells
        VertexGraph = new Graph<Vertex>();
        foreach (Cell cell in CellGraph.GetData())
        {
            for (int i = 0; i < cell.Vertices.Count; i++)
            {
                if (!VertexGraph.Contains(cell.Vertices[i]))
                {
                    VertexGraph.Add(cell.Vertices[i]);
                }
                else
                {
                    // Clean this line up
                    cell.ReplaceVertex(cell.Vertices[i], VertexGraph.GetData()[VertexGraph.GetData().IndexOf(cell.Vertices[i])]);
                }
            }
        }

        Debug.Log(VertexGraph.Count);

        // STEP 7. Establish cell adjacency

        // STEP 8. Establi

        // STEP 9. Relax vertices

    }
}