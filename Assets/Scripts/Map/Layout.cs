using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Structures;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Map
{
    [CreateAssetMenu(fileName = "New Map Layout", menuName = "Map Layout", order = 50)]
    public class Layout : ScriptableObject
    {
        [SerializeField] private int depth;
        [SerializeField] private int seed;

        [SerializeField] private float lineWeight;
        [SerializeField] private float roadWeight;

        [SerializeField] private int relaxIterations;
        [SerializeField] [Range(0f, 1f)] private float relaxStrength;

        [field: SerializeField] public Graph<Vertex> VertexGraph { get; set; }
        [field: SerializeField] private Graph<Cell> CellGraph { get; set; }
        public Graph<Vertex> RoadGraph { get; set; }
        private Dictionary<Cell, List<int>> UVMap { get; set; }

        //Procedurally places buildings
        public IEnumerator FillGrid()
        {
            Debug.Log("Starting Fill Grid: " + Time.time);

            //TODO: Pick a spawn cell from a list of 'safe' placements, avoid building trees in cell within a distance of the hall, Store the cell 
            int count = 0;
            foreach (Cell cell in CellGraph.Data.Where(cell => cell.Active))
            {
                if (cell.Occupied) continue;
                count++;
                Manager.Structures.AddTerrain(cell.Id);
                yield return null;
            }
            Debug.Log("Finish Fill Grid: " + Time.time + ", Added " + count);

            // Play a single build sound for all terrain to avoid "pop"
            if (!Manager.State.Loading) Manager.Jukebox.PlayBuild();
        }
    
        #region Querying
        
        private Vertex RandomBoundaryVertex => VertexGraph.Data.Where(v => v.Boundary).ToList().SelectRandom();

        private List<Vertex> GetVertices(List<Cell> cells)
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach (Cell cell in cells)
                foreach (Vertex vertex in cell.Vertices.Where(vertex => !vertices.Contains(vertex)))
                    vertices.Add(vertex);

            return vertices;
        }
    
        public List<int> GetUVs(Cell cell)
        {
            return UVMap[cell];
        }

        private Cell Step(Cell root, Direction direction)
        {
            Vertex left = root.Vertices[(int)direction];
            Vertex right = root.Vertices[((int)direction + 1) % 4];

            return CellGraph.GetAdjacentData(root.Id).FirstOrDefault(neighbour => 
                neighbour.Active && neighbour.Vertices.Contains(left) && neighbour.Vertices.Contains(right));
        }

        private Cell Step(Cell root, List<Direction> directions, int offset = 0)
        {
            Cell current = root;

            foreach (Direction direction in directions)
            {
                int pivotIndex = ((int)direction + offset) % 4;
                int whatItShouldBe = (pivotIndex + 3) % 4;

                Vertex pivot = current.Vertices[pivotIndex];

                Direction offsetDirection = (Direction)pivotIndex;

                current = Step(current, offsetDirection);
                if (current == null) break;

                int whatItIs = current.Vertices.IndexOf(pivot);

                offset = (offset + (whatItIs - whatItShouldBe + 4) % 4) % 4;
            }

            return current;
        }

        // Set the rotation offset of each cell so that it aligns with its neighbours
        public void Align(List<Cell> cells, int rotation = 0)
        {
            List<Cell> visited = new List<Cell>();
            Queue<Cell> queue = new Queue<Cell>();

            //cells[0].RotateCell(rotation);
            cells[0].Rotation = rotation;

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
                if (other.Vertices.Contains(root.Vertices[(i + root.Rotation) % 4]) && other.Vertices.Contains(root.Vertices[(i + root.Rotation + 1) % 4]))
                {
                    pivot = root.Vertices[(i + root.Rotation) % 4];
                    break;
                }
            }

            int pivotIndexInRoot = root.Vertices.IndexOf(pivot);

            int whatItIs = other.Vertices.IndexOf(pivot);
            int whatItShouldBe = (pivotIndexInRoot + 3) % 4;

            int rotations = (whatItIs + root.Rotation - whatItShouldBe + 4) % 4;

            //other.RotateCell(rotations);
            other.Rotation = rotations;
        }

        public Cell GetCell(int id)
        {
            return CellGraph.GetData(id);
        }

        public List<Cell> GetCells(Vector3 worldPosition, float worldRadius)
        {
            return CellGraph.Data.Where(cell => Vector3.Distance(cell.WorldSpace, worldPosition) < worldRadius).ToList();
        }
        
        public List<Cell> GetCells(List<SectionInfo> structure, int rootId, int rotation = 0)
        {
            return structure.Select(sectionInfo => Step(CellGraph.GetData(rootId), sectionInfo.directions, rotation)).ToList();
        }

        public Cell GetClosest(Vector3 unitPos)
        {
            // Pseudo A* I guess?
            const int CenterCell = 514;
            float curDist = 1000;
            Cell curCell = CellGraph.GetData(CenterCell);
            for (int i = 0; i < 100; i++)
            {
                Cell prevCell = curCell;
                foreach (int x in curCell.Neighbours)
                {
                    float dist = Vector3.Distance(CellGraph.GetData(x).CachedCenter, unitPos);
                    if (dist < curDist)
                    {
                        curDist = dist;
                        curCell = CellGraph.GetData(x);
                    }
                }
                if (prevCell.IsEqual(curCell.Id)) break;
            }

            return curCell;
        }

        [Button("Get All Neighbours")]
        public void GetallNeighbours()
        {
            foreach(Cell c in CellGraph.Data)
            {
                c.Neighbours = CellGraph.GetAdjacent(c.Id);
            }
        }

        [Button("Cache Center")]
        public void CacheCenter()
        {
            foreach (Cell c in CellGraph.Data)
            {
                c.CachedCenter = c.Centre;
            }
        }

        public List<Cell> GetNeighbours(Cell cell)
        {
            return CellGraph.GetAdjacentData(cell.Id);
        }

        #endregion
        #region GridGeneration
        [Button("Regenerate (Warning: Destructive)")] public void Generate()
        {
            HashSet<int> activeCells = new HashSet<int>();
            foreach (Cell cell in CellGraph.Data.Where(cell => cell.Active)) activeCells.Add(cell.Id);

            Random.InitState(seed);
            CreateVertices();
            CreateEdges();
            RemoveEdges();
            Subdivide();
            Relax();
            CalculateCells(activeCells);
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
        private void CalculateCells(HashSet<int> activeCells)
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
                    newCell.Active = activeCells.Contains(newCell.Id) || activeCells.Count == 0;
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
        public Mesh GenerateCellMesh(bool debug)
        {
            RoadGraph = new Graph<Vertex>();
            UVMap = new Dictionary<Cell, List<int>>();
            
            List<Vector3> vertices = new List<Vector3>(CellGraph.Count * 4); // 4 per cells
            List<Vector2> uv = new List<Vector2>(CellGraph.Count * 4); // Match vertices
            List<int> triangles = new List<int>(CellGraph.Count * 6); // 2 triangles per cell

            foreach (Cell cell in CellGraph.Data.Where(cell => cell.Active)) // Only draw active cells 
            {
                // Add the 4 vertices
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(cell.Vertices[i] + (cell.Centre - cell.Vertices[i]).normalized * lineWeight / 100f);
                    uv.Add(debug && cell.WaterFront ? new Vector2(1, 0) : Vector2.zero); // Set to base or invalid if a 'safe' cell
                }
                UVMap.Add(cell, Enumerable.Range( vertices.Count - 4, 4).ToList());

                List<int> trianglesForCell = new List<int>{
                    vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, // Triangle A
                    vertices.Count - 2, vertices.Count - 1, vertices.Count - 4 // Triangle B
                };
                
                // Add the two triangles to both the Mesh and the cell map for highlighting
                triangles.AddRange(trianglesForCell);
            }

            return new Mesh {
                vertices = vertices.ToArray(),
                uv = uv.ToArray(),
                triangles = triangles.ToArray()
            };
        }

        [Button("Set Active Vertices")]
        public void SetVerticesActive()
        {
            VertexGraph.Data.ForEach(vertex => vertex.Active = false);
            CellGraph.Data.ForEach(cell =>
            {
                if (!cell.Active) return;
                cell.Vertices.ForEach(vertex =>
                {
                    vertex.Active = true;
                    VertexGraph.GetData(vertex.Id).Active = true;
                });
            });
        }
        
        #endregion 
        #region Roads
        
        public void CreateRoad(List<Cell> cells, MeshFilter mesh)
        {
            // TODO: Finding the perimeter can be made easier by making a ring around the cells 
            List<Vertex> vertices = GetVertices(cells);
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
            var perimeter = new List<Vertex>();
            
            Algorithms.ConvexHullAsync(vertices, VertexGraph, e => perimeter = e);
            
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
            Grass.GrassEffectController.GrassNeedsUpdate = true;
            //yield return new WaitForEndOfFrame();
        }

        public void ClearRoad(MeshFilter mesh)
        {
            RoadGraph = new Graph<Vertex>();
            mesh.sharedMesh = GenerateRoadMesh();
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

                    Vector3[] normals = {
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                    };

                    int[] triangles = { 0, 1, 2, 2, 1, 3 };

                    Mesh quad = new Mesh {
                        vertices = vertices,
                        uv = uv,
                        triangles = triangles,
                        normals = normals
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

                    Vector3[] normals = {
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                    };

                    int[] triangles = { 0, 1, 2, 2, 1, 3 };

                    Mesh quad = new Mesh {
                        vertices = vertices,
                        uv = uv,
                        triangles = triangles,
                        normals = normals
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

        public List<Vector3> RandomRoadPath
        {
            get
            {
                var path = Algorithms.AStar(RoadGraph, RandomRoadVertex, RandomRoadVertex);
                return path.Count == 0 ? new List<Vector3>() : 
                    path.Select(vertex => Manager.Map.transform.TransformPoint(vertex)).ToList();
            }
        }

        public Vertex RandomRoadVertex
        {
            get
            {
                var verts = Manager.Structures.RandomCell.Vertices
                    .Where(v => RoadGraph.Data.Contains(v)).ToList();
                return verts.Count > 0 ? verts.SelectRandom() : null;
            }
        }

        #endregion
    }
}
