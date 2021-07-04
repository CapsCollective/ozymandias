using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CielaSpike;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Entities
{
    [CreateAssetMenu(fileName = "New Map Layout", menuName = "Map Layout", order = 50)]
    public class MapLayout : ScriptableObject
    {
        [SerializeField] private int depth;
        [SerializeField] private int seed;

        [SerializeField] private float lineWeight;
        [SerializeField] private float roadWeight;

        [SerializeField] private int relaxIterations;
        [SerializeField] [Range(0f, 1f)] private float relaxStrength;

        private Graph<Vertex> VertexGraph { get; set; }
        private Graph<Cell> CellGraph { get; set; }
        private Graph<Vertex> RoadGraph { get; set; }
        private Dictionary<Cell, List<int>> TriangleMap { get; set; }
        private Dictionary<Building, List<Cell>> BuildingMap { get; set; }
    
        public static Action OnRoadReady;

        // BUILDING PLACEMENT
        public void Occupy(Building building, Cell[] cells)
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
            Building building = root.Occupant;

            if (!building) return;
            foreach (Cell cell in BuildingMap[building])
                cell.Clear();

            BuildingMap.Remove(building);

            building.Clear();
        }
    
        // GRID QUERYING
        private Vertex RandomBuildingVertex => BuildingMap[Manager.Buildings.SelectRandom()]
            .SelectMany(c => c.Vertices)
            .Where(v => RoadGraph.Data.Contains(v))
            .ToList().SelectRandom();

        private Vertex RandomBoundaryVertex => VertexGraph.Data.Where(v => v.Boundary).ToList().SelectRandom();
    
        public List<Vertex> GetVertices(Cell[] cells)
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach (Cell cell in cells)
            foreach (Vertex vertex in cell.Vertices.Where(vertex => !vertices.Contains(vertex)))
                vertices.Add(vertex);

            return vertices;
        }
    
        public List<int> GetTriangles(Cell cell)
        {
            return TriangleMap[cell];
        }
    
        private Vertex ClosestRoad(Vertex target)
        {
            if (RoadGraph.Count == 0)
                return null;

            Vertex closest = RoadGraph.Data[0];

            foreach (Vertex vertex in RoadGraph.Data)
            {
                if (Vector3.Distance(target, vertex) < Vector3.Distance(target, closest))
                    closest = vertex;
            }

            return closest;
        }

        private Cell Step(Cell root, Building.Direction direction)
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

        private Cell Step(Cell root, Building.Direction[] directions, int offset = 0)
        {
            Cell current = root;

            foreach (Building.Direction direction in directions)
            {
                int pivotIndex = ((int)direction + offset) % 4;
                int whatItShouldBe = (pivotIndex + 3) % 4;

                Vertex pivot = current.Vertices[pivotIndex];

                Building.Direction offsetDirection = (Building.Direction)pivotIndex;

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

        public Cell[] GetCells(Cell root, Building building, int rotation = 0)
        {
            List<Cell> cells = new List<Cell>();

            foreach (Building.SectionInfo sectionInfo in building.sections)
            {
                Cell newCell = Step(root, sectionInfo.directions.ToArray(), rotation);
                cells.Add(newCell);
            }

            return cells.ToArray();
        }

        public Cell GetClosest(Vector3 unitPos)
        {
            Cell closest = CellGraph.Data[0];
            float minDist = Vector3.Distance(closest.Centre, unitPos);
            foreach (Cell cell in CellGraph.Data)
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

        public Cell[] GetCells(Building building) //Gets all cells a building occupies
        {
            return BuildingMap[building].ToArray();
        }

        // GRID GENERATION
        public Mesh Generate()
        {
            VertexGraph = new Graph<Vertex>();
            CellGraph = new Graph<Cell>();
            RoadGraph = new Graph<Vertex>();
            TriangleMap = new Dictionary<Cell, List<int>>();
            BuildingMap = new Dictionary<Building, List<Cell>>();

            Random.InitState(seed);

            CreateVertices();

            CreateEdges();

            RemoveEdges();

            Subdivide();

            Relax();

            CalculateCells();
        
            return GenerateCellMesh();
        }

        private void CreateVertices()
        {
            // Min and max columns to iterate through
            int minCol = -depth;
            int maxCol = depth;

            float xStep = 2.4f * Mathf.Cos(Mathf.PI / 6f);
            float yStep = 2.4f;

            for (int itCol = minCol; itCol <= maxCol; itCol++)
            {
                int verticesInColumn = (depth * 2 + 1) - Mathf.Abs(itCol);
                float x = xStep * itCol;

                for (int itRow = 0; itRow < verticesInColumn; itRow++)
                {
                    float y = yStep * (itRow - ((verticesInColumn - 1) / 2f));

                    bool boundary = itCol == minCol || itCol == maxCol || itRow == 0 || itRow == verticesInColumn - 1;

                    Vertex newVertex = new Vertex(new Vector3(x, y, 0), false, boundary);

                    VertexGraph.Add(newVertex);
                }
            }
        }

        private void CreateEdges()
        {
            int minCol = -depth;
            int maxCol = depth;

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

        private void RemoveEdges()
        {
            // Creating a new vertex graph based off the current, and remove edges between boundary vertices
            Graph<Vertex> selectionGraph = new Graph<Vertex>(VertexGraph);
            List<Vertex> boundaryNeighbours;

            foreach (Vertex root in selectionGraph.Data)
            {
                boundaryNeighbours = new List<Vertex>();
                foreach (Vertex neighbour in selectionGraph.GetAdjacent(root))
                {
                    if (root.Boundary && neighbour.Boundary) boundaryNeighbours.Add(neighbour);
                }

                foreach (Vertex neighbour in boundaryNeighbours)
                {
                    selectionGraph.DestroyEdge(root, neighbour);
                }
            }

            while (selectionGraph.Count > 0)
            {
                Vertex root = selectionGraph.Data[Random.Range(0, selectionGraph.Count)];
                if (selectionGraph.GetAdjacent(root).Count == 0)
                {
                    selectionGraph.Remove(root);
                    continue;
                }

                Vertex neighbour = selectionGraph.GetAdjacent(root)[Random.Range(0, selectionGraph.GetAdjacent(root).Count)];

                selectionGraph.DestroyEdge(root, neighbour);
                VertexGraph.DestroyEdge(root, neighbour);

                List<Vertex> common = new List<Vertex>();
                foreach (Vertex nRoot in VertexGraph.GetAdjacent(root))
                {
                    foreach (Vertex nNeighbour in VertexGraph.GetAdjacent(neighbour))
                    {
                        if (nRoot == nNeighbour) common.Add(nRoot);
                    }
                }

                foreach (Vertex vertex in common)
                {
                    selectionGraph.DestroyEdge(root, vertex);
                    selectionGraph.DestroyEdge(neighbour, vertex);
                }
            }
        }

        private void Subdivide()
        {
            // Edge Splitting
            Graph<Vertex> splitGraph = new Graph<Vertex>(VertexGraph);
            splitGraph.RemoveEdges();

            foreach (Vertex root in VertexGraph.Data)
            {
                foreach (Vertex neighbour in VertexGraph.GetAdjacent(root))
                {
                    if (splitGraph.HasSharedNeighbours(root, neighbour)) continue;
                    bool boundary = root.Boundary && neighbour.Boundary;
                    
                    Vertex interp = new Vertex((root + neighbour) / 2f, true, boundary);

                    splitGraph.Add(interp);
                    splitGraph.CreateEdge(root, interp);
                    splitGraph.CreateEdge(neighbour, interp);
                }
            }

            // Split Vertex Linking
            Dictionary<Vertex, List<Vertex>> subdivisions = new Dictionary<Vertex, List<Vertex>>();
            int maxDepth = 4;

            foreach (Vertex split in splitGraph.Data)
            {
                if (split.Split)
                {
                    Vertex A = splitGraph.GetAdjacent(split)[0];
                    Vertex B = splitGraph.GetAdjacent(split)[1];

                    List<List<Vertex>> paths = VertexGraph.IndirectDFS(A, B, maxDepth);

                    foreach (List<Vertex> path in paths)
                    {
                        List<Vertex> splits = new List<Vertex>();
                        for (int i = 0; i < path.Count; i++)
                        {
                            splits.Add(splitGraph.SharedNeighbours(path[i], path[(i + 1) % path.Count])[0]);
                        }

                        if (!ListsMatch(splits, subdivisions))
                        {
                            Vector3 divPos = new Vector3();
                            foreach (Vertex splitVert in splits)
                            {
                                divPos += splitVert;
                            }
                            divPos /= splits.Count;

                            subdivisions.Add(new Vertex(divPos, true, false), splits);
                        }
                    }
                }
            }


            foreach (KeyValuePair<Vertex, List<Vertex>> pair in subdivisions)
            {
                splitGraph.Add(pair.Key);
                foreach (Vertex value in pair.Value)
                    splitGraph.CreateEdge(value, pair.Key);
            }

            VertexGraph = splitGraph;
        }

        private void Relax()
        {
            for (int i = 0; i < relaxIterations; i++)
            {
                foreach (Vertex vertex in VertexGraph.Data)
                {
                    if (vertex.Boundary) continue;

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

        private void CalculateCells()
        {
            Graph<Vertex> dupGraph = new Graph<Vertex>(VertexGraph);

            while (dupGraph.Count > 0)
            {
                Vertex root = dupGraph.Data[0];

                // Do the stuff
                List<List<Vertex>> paths = dupGraph.OneWayDFS(root, root, 5);
                foreach (List<Vertex> path in paths)
                {
                    Cell newCell = new Cell(path[0], path[1], path[2], path[3]);
                    if (!CellGraph.Contains(newCell))
                        CellGraph.Add(newCell);
                }

                dupGraph.Remove(root);
            }

            // Establish cell adjacency
            foreach (Cell root in CellGraph.Data)
            {
                foreach (Cell other in CellGraph.Data)
                {
                    int sharedVertices = 0;

                    foreach (Vertex rootVertex in root.Vertices)
                        if (other.Vertices.Contains(rootVertex))
                            sharedVertices++;

                    if (sharedVertices == 2)
                        CellGraph.CreateEdge(root, other);
                }
            }
        }

        private bool ListsMatch<T>(List<T> a, Dictionary<T, List<T>> b)
        {
            return b.Values.Any(bT => ListsMatch(a, bT));
        }

        private bool ListsMatch<T>(List<T> a, List<T> b)
        {
            return a.All(b.Contains);
        }

        // MESH GENERATION
        private Mesh GenerateRoadMesh()
        {
            List<CombineInstance> longs = new List<CombineInstance>();
            List<CombineInstance> corners = new List<CombineInstance>();

            Graph<Vertex> dupGraph = new Graph<Vertex>(RoadGraph);
            for (int i = dupGraph.Count - 1; i >= 0; i--)
            {
                Vertex root = dupGraph.Data[i];
                foreach (Vertex adjacent in dupGraph.GetAdjacent(root))
                {
                    longs.Add(
                        new CombineInstance()
                        {
                            mesh = QuadFromVertices(root, adjacent),
                            transform = Matrix4x4.identity
                        }
                    );
                }

                corners.Add(
                    new CombineInstance()
                    {
                        mesh = QuadFromVertex(root),
                        transform = Matrix4x4.identity
                    }
                );

                dupGraph.RemoveAt(i);
            }

            Mesh longMesh = new Mesh();
            longMesh.CombineMeshes(longs.ToArray());

            Mesh cornerMesh = new Mesh();
            cornerMesh.CombineMeshes(corners.ToArray());

            CombineInstance[] components = {
                new CombineInstance {
                    mesh = longMesh,
                    transform = Matrix4x4.identity
                },
                new CombineInstance {
                    mesh = cornerMesh,
                    transform = Matrix4x4.identity
                }
            };

            Mesh roadMesh = new Mesh();
            roadMesh.CombineMeshes(components, false);

            return roadMesh;
        }

        private Mesh GenerateCellMesh()
        {
            List<Vector3> vertices = new List<Vector3>(CellGraph.Count * 4);
            List<Vector2> uv = new List<Vector2>(CellGraph.Count * 4);
            List<int> triangles = new List<int>(CellGraph.Count * 6);

            foreach (Cell cell in CellGraph.Data)
            {
                // Add the 4 vertices
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(cell.Vertices[i] + (cell.Centre - cell.Vertices[i]).normalized * lineWeight / 100f);
                    uv.Add(Vector2.zero);
                }

                // Add the two triangles to both the Mesh and the Map
                int[] triangleA = { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2 };
                int[] triangleB = { vertices.Count - 2, vertices.Count - 1, vertices.Count - 4 };

                triangles.AddRange(triangleA);
                triangles.AddRange(triangleB);

                // List<int> triangleValue = new List<int>();
                // triangleValue.AddRange(triangleA);
                // triangleValue.AddRange(triangleB);

                TriangleMap.Add(cell, triangles.GetRange(triangles.Count-6, 6));
            }

            return new Mesh {
                vertices = vertices.ToArray(),
                uv = uv.ToArray(),
                triangles = triangles.ToArray()
            };
        }

        private Mesh QuadFromVertex(Vertex centre)
        {
            Vector3 direction = Vector3.up;
            Vector3 cross = Vector3.right;

            Vector3[] vertices = {
                centre - direction * roadWeight / 2f - cross * roadWeight / 2f,
                centre + direction * roadWeight / 2f - cross * roadWeight / 2f,
                centre - direction * roadWeight / 2f + cross * roadWeight / 2f,
                centre + direction * roadWeight / 2f + cross * roadWeight / 2f
            };

            Vector2[] uv = {
                Vector2.zero,
                Vector2.up,
                Vector2.right,
                Vector2.one
            };

            int[] triangles = { 0, 1, 2, 2, 1, 3 };

            Mesh quad = new Mesh {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };

            return quad;
        }

        private Mesh QuadFromVertices(Vertex from, Vertex to)
        {
            Vector3 direction = (to - from).normalized;
            Vector3 cross = Vector3.Cross(direction, Vector3.forward);

            Vector3[] vertices = {
                from - cross * roadWeight / 2f,
                to - cross * roadWeight / 2f,
                from + cross * roadWeight / 2f,
                to + cross * roadWeight / 2f
            };

            Vector2[] uv = {
                Vector2.zero,
                Vector2.up,
                Vector2.right,
                Vector2.one
            };

            int[] triangles = { 0, 1, 2, 2, 1, 3 };

            Mesh quad = new Mesh {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };

            return quad;
        }

        // ROADS
        public IEnumerator CreateRoad(List<Vertex> vertices, MeshFilter mesh)
        {
            // Create a list of vertices included in the building

            // Remove existing intersecting roads
            foreach (Vertex included in vertices)
            {
                if (!RoadGraph.Contains(included)) continue;
                Vertex[] adjacent = RoadGraph.GetAdjacent(included).ToArray();
                for (int i = adjacent.Length - 1; i >= 0; i--)
                {
                    if (vertices.Contains(adjacent[i]))
                        RoadGraph.DestroyEdge(included, adjacent[i]);
                }
            }

            // Create a perimeter path around the included vertices
            List<Vertex> perimeter = new List<Vertex>();
            Algorithms.Runner.StartCoroutineAsync(Algorithms.ConvexHullAsync(vertices, VertexGraph, (e) => perimeter = e), out Task task);
            yield return Algorithms.Runner.StartCoroutine(task.Wait());
            //Debug.Log(perimeter.Count);


            // Create a road linking the perimeter to the existing road graph
            if (RoadGraph.Count > 0 && perimeter.Count > 0)
            {
                Vertex roadStart = perimeter[0];
                Vertex roadTarget = ClosestRoad(roadStart);
                float minDistance = Vector3.Distance(roadStart, roadTarget);

                foreach (Vertex vertex in perimeter)
                {
                    Vertex closest = ClosestRoad(vertex);
                    float distance = Vector3.Distance(closest, vertex);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        roadStart = vertex;
                        roadTarget = closest;
                    }
                }

                List<Vertex> road = Algorithms.AStar(VertexGraph, roadStart, roadTarget);

                // Add the road to the RoadGraph
                AddRoad(road);
            }

            // Add the perimeter to RoadGraph
            AddRoad(perimeter);
            mesh.sharedMesh = GenerateRoadMesh();
            OnRoadReady?.Invoke();
            yield return new WaitForEndOfFrame();
        }

        private void AddRoad(List<Vertex> road)
        {
            for (int i = 0; i < road.Count; i++)
            {
                if (!RoadGraph.Contains(road[i]))
                    RoadGraph.Add(road[i]);

                if (i > 0 && !RoadGraph.IsAdjacent(road[i], road[i - 1]))
                    RoadGraph.CreateEdge(road[i], road[i - 1]);
            }
        }

        public List<Vector3> GetRandomRoadPath()
        {
            return Algorithms.AStar(RoadGraph,RandomBuildingVertex, RandomBuildingVertex)
                .Select(vertex => Manager.Map.transform.TransformPoint(vertex)).ToList();  
        } 
    }
}
