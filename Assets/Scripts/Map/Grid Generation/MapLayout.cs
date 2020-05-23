﻿using System.Collections.Generic;
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
    public Dictionary<BuildingStructure, List<Cell>> BuildingMap = new Dictionary<BuildingStructure, List<Cell>>();

    // BUILDING PLACEMENT
    public void Occupy(BuildingStructure building, Cell[] cells)
    {
        if (!BuildingMap.ContainsKey(building))
            BuildingMap.Add(building, new List<Cell>());

        foreach (Cell cell in cells)
        {
            if (!BuildingMap[building].Contains(cell))
                BuildingMap[building].Add(cell);

            cell.Occupy(building);
        }
    }

    public void Clear(Cell root)
    {
        BuildingStructure building = root.occupant;

        if (building)
        {
            foreach (Cell cell in BuildingMap[building])
                cell.Clear();

            BuildingMap.Remove(building);
        }
    }

    // GRID QUERYING
    public Cell Step(Cell root, BuildingStructure.Direction direction)
    {
        Cell next = null;

        Vertex left = root.Vertices[(int)direction];
        Vertex right = root.Vertices[((int)direction + 1) % 4];

        foreach (Cell neighbour in CellGraph.GetAdjacent(root))
        {
            if (neighbour.Vertices.Contains(left) && neighbour.Vertices.Contains(right))
            {
                next = neighbour;
                break;
            }
        }

        return next;
    }

    public Cell Step(Cell root, BuildingStructure.Direction[] directions, int offset = 0)
    {
        Cell current = root;

        foreach (BuildingStructure.Direction direction in directions)
        {
            int pivotIndex = ((int)direction + offset) % 4;
            int whatItShouldBe = (pivotIndex + 3) % 4;

            Vertex pivot = current.Vertices[pivotIndex];

            BuildingStructure.Direction offsetDirection = (BuildingStructure.Direction)pivotIndex;

            current = Step(current, offsetDirection);
            if (current == null) break;

            int whatItIs = current.Vertices.IndexOf(pivot);

            offset = (offset + ((whatItIs - whatItShouldBe + 4) % 4)) % 4;
        }

        return current;
    }

    public void Align(Cell[] cells, int rotation = 0)
    {
        List<Cell> visited = new List<Cell>();
        Queue<Cell> queue = new Queue<Cell>();

        cells[0].RotateCell(rotation);

        queue.Enqueue(cells[0]);
        while (queue.Count > 0)
        {
            Cell root = queue.Dequeue();
            foreach (Cell other in cells)
            {
                if (!visited.Contains(other) && CellGraph.IsAdjacent(root, other) && !queue.Contains(other))
                {
                    Align(root, other);
                    
                    queue.Enqueue(other);
                }
            }
            visited.Add(root);
        }
    }

    private void Align(Cell root, Cell other)
    {
        Vertex pivot = root.Vertices[0];
        for (int i = 0; i < 4; i++)
        {
            if (other.Vertices.Contains(root.Vertices[i]) && other.Vertices.Contains(root.Vertices[(i + 1) % 4]))
            {
                pivot = root.Vertices[i];
                break;
            }
        }

        int pivotIndexInRoot = root.Vertices.IndexOf(pivot);

        int whatItIs = other.Vertices.IndexOf(pivot);
        int whatItShouldBe = (pivotIndexInRoot + 3) % 4;

        int rotations = (whatItIs - whatItShouldBe + 4) % 4;

        other.RotateCell(rotations);
    }

    public Cell[] GetCells(Cell root, BuildingStructure building, int rotation = 0)
    {
        List<Cell> cells = new List<Cell>();

        foreach (BuildingStructure.SectionInfo sectionInfo in building.sections)
        {
            Cell newCell = Step(root, sectionInfo.directions.ToArray(), rotation);
            cells.Add(newCell);
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

    public Cell[] GetCells(BuildingStructure building) //Gets all cells a building occupies
    {
        return BuildingMap[building].ToArray();
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