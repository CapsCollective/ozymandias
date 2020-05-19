using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Layout", menuName = "Map Layout", order = 50)]
public class MapLayout : ScriptableObject
{
    public int depth;
    public int seed;

    public float lineWeight;

    public int relaxIterations;
    [Range(0f, 1f)] public float relaxStrength;

    public Graph<Vertex> VertexGraph { get; private set; } = new Graph<Vertex>();
    public Graph<Triangle> TriangleGraph { get; private set; } = new Graph<Triangle>();
    public Graph<Cell> CellGraph { get; private set; } = new Graph<Cell>();
    public Dictionary<Cell, List<int>> TriangleMap = new Dictionary<Cell, List<int>>();

    // GRID QUERYING
    public Cell Step(Cell root, int direction)
    {
        Cell step = null;

        Vertex left = root.Vertices[direction];
        Vertex right = root.Vertices[(direction + 1) % 4];

        foreach (Cell neighbour in CellGraph.GetAdjacent(root))
        {
            if (!neighbour.Occupied && neighbour.Vertices.Contains(left) && neighbour.Vertices.Contains(right))
            {
                step = neighbour;

                while (neighbour.Vertices.IndexOf(left) != direction)
                {
                    Vertex end = neighbour.Vertices[3];
                    neighbour.Vertices.Remove(end);
                    neighbour.Vertices.Insert(0, end);
                }

                break;
            }
        }

        return step;
    }

    //public Cell[] GetCells(BuildingPlacement.Building building, Vector3 unitPosition)
    //{
    //    List<Cell> cells = new List<Cell>();
    //    Cell root = GetClosest(unitPosition);

    //    foreach (BuildingPlacement.Building.SectionInfo sectionInfo in building.sections)
    //    {
    //        Cell newCell = root;
    //        foreach (BuildingPlacement.Building.Direction direction in sectionInfo.directions)
    //        {
    //            newCell = Step(newCell, (int)direction);
    //            if (newCell == null) return null;
    //        }

    //        if (!newCell.Occupied)
    //            cells.Add(newCell);
    //        else
    //            return null;
    //    }

    //    return cells.ToArray();
    //}

    public Cell[] GetCells(Cell root, BuildingPlacement.Building building)
    {
        List<Cell> cells = new List<Cell>();

        foreach (BuildingPlacement.Building.SectionInfo sectionInfo in building.sections)
        {
            Cell newCell = root;
            foreach (BuildingPlacement.Building.Direction direction in sectionInfo.directions)
            {
                newCell = Step(newCell, (int)direction);
                if (newCell == null) break;
            }

            cells.Add(newCell);
        }

        return cells.ToArray();
    }

    public Cell[] GetClosestUnoccupied(Vector3 unitPos, BuildingMesh bMesh)
    {
        Cell closest = GetClosest(unitPos);
        if (closest.Occupied) return null;

        List<Cell> cells = new List<Cell>() { closest };
        foreach (BuildingMesh.Extension extension in bMesh.extensions)
        {
            Cell extensionCell = closest;
            foreach (BuildingMesh.StepDirection step in extension.steps)
            {
                extensionCell = Step(extensionCell, (int)step);
                if (extensionCell.Occupied) return null;
                cells.Add(extensionCell);
            }
        }

        return cells.ToArray();
    }

    public Cell GetClosest(Vector3 unitPos)
    {
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

        return closest;
    }

    // GRID GENERATION
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

        // STEP 7. Establish cell adjacency
        foreach (Cell root in CellGraph.GetData())
        {
            foreach (Cell other in CellGraph.GetData())
            {
                int sharedVertices = 0;

                foreach (Vertex rootVertex in root.Vertices)
                    if (other.Vertices.Contains(rootVertex))
                        sharedVertices++;

                if (sharedVertices == 2)
                    CellGraph.CreateEdge(root, other);
            }
        }

        // STEP 8. Establish vertex adjacency - two vertices are adjacent if they are different and share two or more cells (cellmates ha)
        foreach (Vertex root in VertexGraph.GetData())
        {
            foreach (Vertex other in VertexGraph.GetData())
            {
                int sharedCells = 0;

                foreach (Cell cell in CellGraph.GetData())
                {
                    if (cell.Vertices.Contains(root) && cell.Vertices.Contains(other))
                        sharedCells++;
                }

                if (root != other && sharedCells >= 2)
                {
                    VertexGraph.CreateEdge(root, other);
                }
            }
        }

        // STEP 9. Relax vertices
        for (int i = 0; i < relaxIterations; i++)
        {
            foreach (Vertex vertex in VertexGraph.GetData())
            {
                Vector3 averagedPosition = vertex;

                foreach (Vertex neighbour in VertexGraph.GetAdjacent(vertex))
                {
                    averagedPosition += neighbour;
                }

                averagedPosition /= VertexGraph.GetAdjacent(vertex).Count + 1;

                if (VertexGraph.GetAdjacent(vertex).Count + 1 > 3)
                    vertex.SetPosition(Vector3.Lerp(vertex, averagedPosition, relaxStrength));
            }
        }
    }

    // MESH GENERATION
    public Mesh GenerateCellMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        foreach (Cell cell in CellGraph.GetData())
        {
            // Add the 4 vertices
            vertices.Add(cell.Vertices[0] + (cell.Centre - cell.Vertices[0]).normalized * lineWeight / 100f);
            vertices.Add(cell.Vertices[1] + (cell.Centre - cell.Vertices[1]).normalized * lineWeight / 100f);
            vertices.Add(cell.Vertices[2] + (cell.Centre - cell.Vertices[2]).normalized * lineWeight / 100f);
            vertices.Add(cell.Vertices[3] + (cell.Centre - cell.Vertices[3]).normalized * lineWeight / 100f);

            // Add the two triangles to both the Mesh and the Map
            int[] triangleA = new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2 };
            int[] triangleB = new int[] { vertices.Count - 2, vertices.Count - 1, vertices.Count - 4 };

            triangles.AddRange(triangleA);
            triangles.AddRange(triangleB);

            List<int> triangleValue = new List<int>();
            triangleValue.AddRange(triangleA);
            triangleValue.AddRange(triangleB);

            TriangleMap.Add(cell, triangleValue);

            // Set the uv's
            uv.Add(Vector2.zero);
            uv.Add(Vector2.zero);
            uv.Add(Vector2.zero);
            uv.Add(Vector2.zero);
        }

        return new Mesh()
        {
            vertices = vertices.ToArray(),
            uv = uv.ToArray(),
            triangles = triangles.ToArray()
        };
    }

    public Mesh GenerateEdgeMesh()
    {
        Graph<Vertex> dupVertexGraph = new Graph<Vertex>(VertexGraph);

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int i = dupVertexGraph.Count - 1; i >= 0; i--)
        {
            foreach (Vertex neighbour in dupVertexGraph.GetAdjacent(dupVertexGraph.GetData()[i]))
            {
                Vector3 root = dupVertexGraph.GetData()[i];
                Vector3 toNeighbour = neighbour - root;
                Vector3 direction = toNeighbour.normalized;
                Vector3 cross = Vector3.Cross(direction, Vector3.forward);

                vertices.AddRange(
                    new Vector3[]
                    {
                        root - cross * lineWeight / 2f,
                        neighbour - cross * lineWeight / 2f,
                        root + cross * lineWeight / 2f,
                        neighbour + cross * lineWeight / 2f
                    }
                    );

                uv.AddRange(
                    new Vector2[]
                    {
                        new Vector2(0, 0),
                        new Vector2(0, 1),
                        new Vector2(1, 0),
                        new Vector2(1, 1)
                    }
                    );

                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 1);
            }

            dupVertexGraph.Remove(dupVertexGraph.GetData()[i]);
        }

        return new Mesh()
        {
            vertices = vertices.ToArray(),
            uv = uv.ToArray(),
            triangles = triangles.ToArray()
        };
    }
}