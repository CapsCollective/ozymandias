﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CielaSpike;
using NaughtyAttributes;
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

        [field: HideInInspector] [field: SerializeField] private Graph<Vertex> VertexGraph { get; set; }
        [field: SerializeField] private Graph<Cell> CellGraph { get; set; }
        private Graph<Vertex> RoadGraph { get; set; }
        private Dictionary<Cell, List<int>> TriangleMap { get; set; }
        
        // TODO: Move this and the occupy script into map
        private Dictionary<Building, List<Cell>> BuildingMap { get; set; }
    
        public static Action OnRoadReady;

        // BUILDING PLACEMENT
        public void Occupy(Building building, List<Cell> cells)
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
    
        // Querying
        private Vertex RandomBuildingVertex => BuildingMap[Manager.Buildings.SelectRandom()]
            .SelectMany(c => c.Vertices)
            .Where(v => RoadGraph.Data.Contains(v))
            .ToList().SelectRandom();

        private Vertex RandomBoundaryVertex => VertexGraph.Data.Where(v => v.Boundary).ToList().SelectRandom();
    
        public List<Vertex> GetVertices(List<Cell> cells)
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
            Vertex left = root.Vertices[(int)direction];
            Vertex right = root.Vertices[((int)direction + 1) % 4];

            return CellGraph.GetAdjacentData(root.Id).FirstOrDefault(neighbour => 
                neighbour.Active && neighbour.Vertices.Contains(left) && neighbour.Vertices.Contains(right));
        }

        private Cell Step(Cell root, List<Building.Direction> directions, int offset = 0)
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

                offset = (offset + (whatItIs - whatItShouldBe + 4) % 4) % 4;
            }

            return current;
        }

        public void Align(List<Cell> cells, int rotation = 0)
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
                    if (visited.Contains(other) || !CellGraph.IsAdjacent(root.Id, other.Id) || queue.Contains(other)) continue;
                    Align(root, other);
                    
                    queue.Enqueue(other);
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

        public List<Cell> GetCells(Cell root, Building building, int rotation = 0)
        {
            return building.sections.Select(sectionInfo => Step(root, sectionInfo.directions, rotation)).ToList();
        }

        public Cell GetClosest(Vector3 unitPos)
        {
            const float maxDist = 1f;
            Cell closest = null;
            float minDist = float.MaxValue;
            foreach (Cell cell in CellGraph.Data)
            {
                float distance;
                if (!((distance = Vector3.Distance(unitPos, cell.Centre)) < minDist)) continue;
                minDist = distance;
                closest = cell;
            }

            return minDist < maxDist ? closest : null;
        }

        public List<Cell> GetCells(Building building) //Gets all cells a building occupies
        {
            return BuildingMap[building];
        }

        // Grid
        [Button("Regenerate")] public void Generate()
        {
            Random.InitState(seed);
            CreateVertices();
            CreateEdges();
            RemoveEdges();
            Subdivide();
            Relax();
            CalculateCells();
        }

        // Create vertices in a grid
        private void CreateVertices()
        {
            VertexGraph = new Graph<Vertex>();
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

                    newVertex.Id = VertexGraph.Add(newVertex);
                }
            }
        }

        // Join all adjacent vertices
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
                        VertexGraph.AddEdge(vertexIndex, vertexIndex + 1);
                    }
                    if (itCol < 0)
                    {
                        VertexGraph.AddEdge(vertexIndex, vertexIndex + verticesInColumn + 1);
                        VertexGraph.AddEdge(vertexIndex, vertexIndex + verticesInColumn);
                    }
                    if (itCol > 0)
                    {
                        VertexGraph.AddEdge(vertexIndex, vertexIndex - verticesInColumn);
                        VertexGraph.AddEdge(vertexIndex, vertexIndex - verticesInColumn - 1);
                    }

                    vertexIndex++;
                }
            }
        }

        // Create a new vertex graph based off the current, and remove edges between boundary vertices,
        // keep removing random neighbours until none remain.
        // Still not sure why, I think it's to create variance in the grid so when it's relaxed it gets pulled around
        private void RemoveEdges()
        {
            Graph<Vertex> selectionGraph = new Graph<Vertex>(VertexGraph);

            // Remove boundary edges (outer ring)
            foreach (Vertex root in selectionGraph.Data)
            {
                List<Vertex> boundaryNeighbours = selectionGraph.GetAdjacentData(root.Id).Where(neighbour => root.Boundary && neighbour.Boundary).ToList();

                foreach (Vertex neighbour in boundaryNeighbours)
                {
                    selectionGraph.RemoveEdge(root.Id, neighbour.Id);
                }
            }

            while (selectionGraph.Count > 0)
            {
                int root = selectionGraph.Ids[Random.Range(0, selectionGraph.Count)];
                List<int> adjacent = selectionGraph.GetAdjacent(root);
                
                if (adjacent.Count == 0)
                {
                    selectionGraph.Remove(root);
                    continue;
                }

                int neighbour = adjacent[Random.Range(0, adjacent.Count)];

                selectionGraph.RemoveEdge(root, neighbour);
                VertexGraph.RemoveEdge(root, neighbour);

                List<int> common = (
                    from nRoot in VertexGraph.GetAdjacent(root)
                    from nNeighbour in VertexGraph.GetAdjacent(neighbour)
                    where nRoot == nNeighbour select nRoot
                ).ToList();
                
                foreach (int vertex in common)
                {
                    selectionGraph.RemoveEdge(root, vertex);
                    selectionGraph.RemoveEdge(neighbour, vertex);
                }
            }
        }

        // Splitting the graph by creating a midpoint between
        private void Subdivide()
        {
            // Edge Splitting
            Graph<Vertex> splitGraph = new Graph<Vertex>(VertexGraph);
            splitGraph.RemoveAllEdges();

            foreach (Vertex root in VertexGraph.Data)
            {
                foreach (Vertex neighbour in VertexGraph.GetAdjacentData(root.Id))
                {
                    if (splitGraph.HasSharedNeighbours(root.Id, neighbour.Id)) continue;
                    bool boundary = root.Boundary && neighbour.Boundary;
                    
                    Vertex midpoint = new Vertex((root + neighbour) / 2f, true, boundary);

                    midpoint.Id = splitGraph.Add(midpoint);
                    splitGraph.AddEdge(root.Id, midpoint.Id);
                    splitGraph.AddEdge(neighbour.Id, midpoint.Id);
                }
            }

            // Split Vertex Linking
            Dictionary<Vertex, List<Vertex>> subdivisions = new Dictionary<Vertex, List<Vertex>>();

            foreach (Vertex split in splitGraph.Data)
            {
                if (!split.Split) continue;
                List<int> adjacent = splitGraph.GetAdjacent(split.Id);

                List<List<int>> paths = VertexGraph.Search(adjacent[0], adjacent[1], 4, false);

                foreach (List<int> path in paths)
                {
                    List<Vertex> splits = path.Select((t, i) => splitGraph.SharedNeighbours(t, path[(i + 1) % path.Count])[0]).ToList();

                    if (splits.IsContainedWithin(subdivisions)) continue;
                    
                    Vector3 divPos = new Vector3();
                    divPos = splits.Aggregate(divPos, (current, splitVert) => current + splitVert) / splits.Count;

                    subdivisions.Add(new Vertex(divPos, true, false), splits);
                }
            }


            foreach (KeyValuePair<Vertex, List<Vertex>> subdivision in subdivisions)
            {
                subdivision.Key.Id = splitGraph.Add(subdivision.Key);
                foreach (Vertex vertex in subdivision.Value)
                    splitGraph.AddEdge(vertex.Id, subdivision.Key.Id);
            }

            VertexGraph = splitGraph;
        }

        // Shift all vertices towards their neighbours to smooth out the grid
        private void Relax()
        {
            for (int i = 0; i < relaxIterations; i++)
            {
                foreach (Vertex vertex in VertexGraph.Data)
                {
                    if (vertex.Boundary) continue;

                    // Find the midpoint of all neighbours
                    List<Vertex> neighbours = VertexGraph.GetAdjacentData(vertex.Id);
                    Vector3 averagedPosition = neighbours.Aggregate<Vertex, Vector3>(vertex, (current, neighbour) => current + neighbour) / (neighbours.Count + 1);
                    
                    //TODO: Check why this is here, could we make the edge wavy with this?
                    //if (neighbours.Count + 1 > 3)
                        vertex.SetPosition(Vector3.Lerp(vertex, averagedPosition, relaxStrength));
                }
            }
        }

        // Form a graph of cells by grouping the vertices, then create edges by comparing shared vertices
        private void CalculateCells()
        {
            CellGraph = new Graph<Cell>();
            Graph<Vertex> dupGraph = new Graph<Vertex>(VertexGraph);

            // Go through each vertex in the graph to form cells
            while (dupGraph.Count > 0)
            {
                int root = dupGraph.Ids[0];

                // Form rings of 4 vertices with a search
                List<List<int>> paths = dupGraph.Search(root, root, 5, true);
                foreach (Cell newCell in paths
                    .Select(path => new Cell(path.Select(id => VertexGraph.GetData(id)).ToList()))
                    .Where(newCell => !CellGraph.Contains(newCell)))
                {
                    newCell.Id = CellGraph.Add(newCell);
                }

                dupGraph.Remove(root);
            }

            // Establish cell adjacency by cells with 2 joining vertices
            // Compare cell with all in front of it
            List<Cell> data = CellGraph.Data;
            for (int root = 0; root < data.Count; root++)
                for (int other = root + 1; other < data.Count; other++)
                    if (data[root].Vertices.Count(rootVertex => data[other].Vertices.Contains(rootVertex)) == 2)
                        CellGraph.AddEdge(data[root].Id, data[other].Id);
        }

        // Converts Cells into triangles and returns combined mesh
        public Mesh GenerateCellMesh()
        {
            RoadGraph = new Graph<Vertex>();
            TriangleMap = new Dictionary<Cell, List<int>>();
            BuildingMap = new Dictionary<Building, List<Cell>>();
            
            List<Vector3> vertices = new List<Vector3>(CellGraph.Count * 4); // 4 per cells
            List<Vector2> uv = new List<Vector2>(CellGraph.Count * 4); // Match vertices
            List<int> triangles = new List<int>(CellGraph.Count * 6); // 2 triangles per cell

            foreach (Cell cell in CellGraph.Data.Where(cell => cell.Active)) // Only draw active cells 
            {
                // Add the 4 vertices
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(cell.Vertices[i] + (cell.Centre - cell.Vertices[i]).normalized * lineWeight / 100f);
                    uv.Add(Vector2.zero); // Get set by later methods to highlight cells
                }

                
                List<int> trianglesForCell = new List<int>{
                    vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, // Triangle A
                    vertices.Count - 2, vertices.Count - 1, vertices.Count - 4 // Triangle B
                };
                
                // Add the two triangles to both the Mesh and the cell map for highlighting
                triangles.AddRange(trianglesForCell);
                TriangleMap.Add(cell, trianglesForCell);
            }

            return new Mesh {
                vertices = vertices.ToArray(),
                uv = uv.ToArray(),
                triangles = triangles.ToArray()
            };
        }
        
        // Roads
        //TODO: See if we can convert this whole thing (AStar included) to use ids instead of vertices
        public IEnumerator CreateRoad(List<Vertex> vertices, MeshFilter mesh)
        {
            List<int> vertexIds = vertices.Select(v => v.Id).ToList();
            // Create a list of vertices included in the building

            // Remove existing intersecting roads
            foreach (int included in vertexIds)
            {
                if (!RoadGraph.Contains(included)) continue;
                List<int> adjacent = RoadGraph.GetAdjacent(included);
                for (int i = adjacent.Count - 1; i >= 0; i--)
                {
                    if (vertexIds.Contains(adjacent[i]))
                        RoadGraph.RemoveEdge(included, adjacent[i]);
                }
            }

            // Create a perimeter path around the included vertices
            List<Vertex> perimeter = new List<Vertex>();
            Algorithms.Runner.StartCoroutineAsync(Algorithms.ConvexHullAsync(vertices, VertexGraph, e => perimeter = e), out Task task);
            yield return Algorithms.Runner.StartCoroutine(task.Wait());

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

                    if (distance > minDistance) continue;
                    minDistance = distance;
                    roadStart = vertex;
                    roadTarget = closest;
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
                if (!RoadGraph.Contains(road[i].Id))
                    RoadGraph.Add(road[i], road[i].Id);

                if (i > 0 && !RoadGraph.IsAdjacent(road[i].Id, road[i - 1].Id))
                    RoadGraph.AddEdge(road[i].Id, road[i - 1].Id);
            }
        }
        
        private Mesh GenerateRoadMesh()
        {
            List<CombineInstance> longs = new List<CombineInstance>();
            List<CombineInstance> corners = new List<CombineInstance>();

            Graph<Vertex> dupGraph = new Graph<Vertex>(RoadGraph);
            for (int i = dupGraph.Count - 1; i >= 0; i--)
            {
                Vertex root = dupGraph.Data[i];
                longs.AddRange(dupGraph.GetAdjacentData(root.Id).Select(adjacent => new CombineInstance
                {
                    mesh = QuadFromVertices(root, adjacent), 
                    transform = Matrix4x4.identity
                }));

                corners.Add(
                    new CombineInstance()
                    {
                        mesh = QuadFromVertex(root),
                        transform = Matrix4x4.identity
                    }
                );

                dupGraph.Remove(root.Id);
                
                Mesh QuadFromVertex(Vertex centre)
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

                Mesh QuadFromVertices(Vertex from, Vertex to)
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

        public List<Vector3> GetRandomRoadPath()
        {
            return Algorithms.AStar(RoadGraph,RandomBuildingVertex, RandomBuildingVertex)
                .Select(vertex => Manager.Map.transform.TransformPoint(vertex)).ToList();  
        } 
    }
}
