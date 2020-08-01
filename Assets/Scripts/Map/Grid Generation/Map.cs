using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Map : MonoBehaviour
{
    public MapLayout mapLayout;
    public MeshFilter roadMF;

    [Header("Debug Settings")]
    //public bool selectingVertex;
    //public int selectedVertex;
    //public int selectedCell;
    public bool debug;
    //public float vertexRadius;
    //public Color selectedColor;
    //public Color adjacentColor;
    //public Color vertexColor;
    //public Color gridColor;
    //public Color occupiedColor;
    public LayerMask layerMask;

    private MeshFilter gridMF;
    private Camera cam;
    private Vector2[] uv;

    public enum HighlightState { Inactive, Valid, Invalid }

    private void Awake()
    {
        cam = Camera.main;
        Generate();
        mapLayout.ClearGraph();
    }

    private void AddRoad(Mesh road)
    {
        CombineInstance[] longs = new CombineInstance[]
        {
            new CombineInstance() // longs from existing road mesh
            {
                mesh = roadMF.mesh,
                transform = Matrix4x4.identity,
                subMeshIndex = 0
            },
            new CombineInstance() // longs from new road mesh
            {
                mesh = road,
                transform = roadMF.transform.localToWorldMatrix * transform.worldToLocalMatrix,
                subMeshIndex = 0
            }
        };

        CombineInstance[] corners = new CombineInstance[]
        {
            new CombineInstance() // corners from existing road mesh
            {
                mesh = roadMF.mesh,
                transform = Matrix4x4.identity,
                subMeshIndex = 1
            },
            new CombineInstance() // corners from new road mesh
            {
                mesh = road,
                transform = roadMF.transform.localToWorldMatrix * transform.worldToLocalMatrix,
                subMeshIndex = 1
            }
        };

        roadMF.sharedMesh = new Mesh();

        Mesh longMesh = new Mesh();
        longMesh.CombineMeshes(longs, true);

        Mesh cornerMesh = new Mesh();
        cornerMesh.CombineMeshes(corners, true);

        CombineInstance[] components = new CombineInstance[]
        {
            new CombineInstance()
            {
                mesh = longMesh,
                transform = Matrix4x4.identity
            },
            new CombineInstance()
            {
                mesh = cornerMesh,
                transform = Matrix4x4.identity
            }
        };

        roadMF.sharedMesh.CombineMeshes(components, false);
    }

    //public void GenerateRoad(Vertex from, Vertex to)
    //{
    //    AddRoad(mapLayout.GenerateRoad(from, to, 2000));
    //}

    //public void GenerateRoad(List<Vertex> path)
    //{
    //    AddRoad(mapLayout.GenerateRoad(path));
    //}

    public void Highlight(Cell[] cells, HighlightState state)
    {
        if(uv == null)
            uv = gridMF.sharedMesh.uv;

        foreach (Cell cell in cells)
        {
            if (cell == null)
                continue;
            foreach (int vertexIndex in mapLayout.TriangleMap[cell])
                uv[vertexIndex].x = (int)state / 2f;
        }

        gridMF.sharedMesh.uv = uv;
    }

    // Gets the closest cell to the cursor
    public Cell GetCellFromMouse()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 200f, layerMask);
        return GetCell(hit.point);
    }

    // Gets the closest cell by world position
    public Cell GetCell(Vector3 worldPosition)
    {
        return mapLayout.GetClosest(transform.InverseTransformPoint(worldPosition));
    }

    // Gets all cells within radius of a world position
    public Cell[] GetCells(Vector3 worldPosition, float worldRadius)
    {
        // TODO: Implement
        return new Cell[0];
    }

    // Gets all cells a building would take up given its root and rotation
    public Cell[] GetCells(Cell root, BuildingStructure building, int rotation = 0)
    {
        return mapLayout.GetCells(root, building, rotation);
    }

    // Gets all cells of a currently placed building
    public Cell[] GetCells(BuildingStructure building)
    {
        return mapLayout.GetCells(building);
    }

    public void Generate()
    {
        mapLayout.GenerateMap(mapLayout.seed);
        gridMF = GetComponent<MeshFilter>();
        gridMF.sharedMesh = mapLayout.GenerateCellMesh();
        roadMF.sharedMesh = new Mesh();
    }

    // Occupies and fits a building onto the map
    private void Occupy(BuildingStructure building, Cell[] cells, bool animate = false)
    {
        Vector3[][] vertices = new Vector3[cells.Length][];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = CellUnitToWorld(cells[i]);
        }

        mapLayout.Occupy(building, cells);

        building.Fit(vertices, mapLayout.heightFactor, animate);
    }

    // Tries to create and place a building from a world position and rotation, returns if successful
    public bool CreateBuilding(GameObject buildingPrefab, Vector3 worldPosition, int rotation = 0, bool animate = true, bool placeRoads = true)
    {
        GameObject buildingInstance = Instantiate(buildingPrefab, GameObject.Find("Buildings").transform);
        BuildingStats stats = buildingInstance.GetComponent<BuildingStats>();
        BuildingStructure building = buildingInstance.GetComponent<BuildingStructure>();
        PlaceTerrain placeTerrain = buildingInstance.GetComponent<PlaceTerrain>();
        if (placeTerrain) placeTerrain.rotation = rotation;

        Cell root = GetCell(worldPosition);
        Cell[] cells = GetCells(root, building, rotation);

        Vector3 centre = new Vector3();
        int cellCount = 0;
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                centre += cell.Centre;
                cellCount++;
            }
        }
        centre /= cellCount;

        buildingInstance.transform.position = transform.TransformPoint(centre);

        if (IsValid(cells) && Manager.Spend(stats.ScaledCost))
        {
            if (placeRoads)
            {
                var vertices = mapLayout.GetVertices(cells);

                mapLayout.CreateRoad(vertices);
                roadMF.sharedMesh = mapLayout.GenerateRoadMesh();
            }

            mapLayout.Align(cells, rotation);
            Occupy(building, cells, animate);
            stats.Build();
            return true;
        }

        Destroy(buildingInstance);
        return false;
    }

    public bool IsValid(Cell[] cells)
    {
        bool valid = true;

        for (int i = 0; valid && i < cells.Length; i++)
            valid = IsValid(cells[i]);

        return valid;
    }

    public bool IsValid(Cell cell)
    {
        return cell != null && !cell.Occupied;
    }

    public void Clear(BuildingStructure building)
    {
        Clear(GetCells(building)[0]); // Destroys the building from its root
    }

    public void Clear(Cell[] cells)
    {
        foreach (Cell cell in cells)
            Clear(cell);
    }

    public void Clear(Cell root)
    {
        mapLayout.Clear(root);
    }

    public Vector3[] CellUnitToWorld(Cell cell)
    {
        Vector3[] vertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = transform.TransformPoint(cell.Vertices[i]);
        }
        return vertices;
    }

    private void OnDrawGizmos()
    {
        if (mapLayout && debug)
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = Color.white;
            foreach (Vertex root in mapLayout.RoadGraph.GetData())
            {
                Gizmos.DrawSphere(root, .003f);
                foreach (Vertex neighbour in mapLayout.RoadGraph.GetAdjacent(root))
                {
                    Gizmos.DrawLine(root, neighbour);
                }
            }

            //Gizmos.color = vertexColor;

            //foreach (Vertex root in mapLayout.VertexGraph.GetData())
            //{
            //    Gizmos.DrawSphere(root, vertexRadius);
            //}

            //Gizmos.color = gridColor;
            //foreach (Cell cell in mapLayout.CellGraph.GetData())
            //{
            //    cell.DrawCell();
            //}

            //if (selectingVertex && mapLayout.VertexGraph.Count > 0)
            //{
            //    selectedVertex = Mathf.Clamp(selectedVertex, 0, mapLayout.VertexGraph.Count - 1);

            //    Gizmos.color = selectedColor;
            //    Gizmos.DrawSphere(mapLayout.VertexGraph.GetData()[selectedVertex], vertexRadius);

            //    Gizmos.color = adjacentColor;
            //    foreach (Vertex adjacent in mapLayout.VertexGraph.GetAdjacent(mapLayout.VertexGraph.GetData()[selectedVertex]))
            //        Gizmos.DrawSphere(adjacent, vertexRadius);
            //}
            //else if (mapLayout.CellGraph.Count > 0)
            //{
            //    selectedCell = Mathf.Clamp(selectedCell, 0, mapLayout.CellGraph.Count - 1);

            //    Gizmos.color = adjacentColor;
            //    foreach (Cell adjacent in mapLayout.CellGraph.GetAdjacent(mapLayout.CellGraph.GetData()[selectedCell]))
            //        adjacent.DrawCell();

            //    Gizmos.color = selectedColor;
            //    mapLayout.CellGraph.GetData()[selectedCell].DrawCell();
            //    for (int i = 0; i < 4; i++)
            //    {
            //        Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)i / 3);
            //        Gizmos.DrawSphere(mapLayout.CellGraph.GetData()[selectedCell].Vertices[i], vertexRadius);
            //    }
            //}

            //Gizmos.color = occupiedColor;
            //foreach (Cell cell in mapLayout.CellGraph.GetData())
            //{
            //    if (cell.Occupied) cell.DrawCell();
            //}
        }
    }
}
