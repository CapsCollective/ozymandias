using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Map : MonoBehaviour
{
    public MapLayout mapLayout;

    [Header("Debug Settings")]
    public bool selectingVertex;
    public int selectedVertex;
    public int selectedCell;
    public bool debug;
    public float vertexRadius;
    public Color selectedColor;
    public Color adjacentColor;
    public Color vertexColor;
    public Color gridColor;
    public Color occupiedColor;

    private MeshFilter _meshFilter;
    private Camera cam;

    public enum HighlightState { Inactive, Valid, Invalid }

    private void Awake()
    {
        cam = Camera.main;
        Generate();
    }

    public void Highlight(Cell[] cells, HighlightState state)
    {
        Vector2[] uv = _meshFilter.sharedMesh.uv;

        foreach (Cell cell in cells)
        {
            if (cell == null)
                continue;
            foreach (int vertexIndex in mapLayout.TriangleMap[cell])
                uv[vertexIndex].x = (int)state / 2f;
        }

        _meshFilter.sharedMesh.uv = uv;
    }

    // Gets the closest cell to the cursor
    public Cell GetCellFromMouse()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, LayerMask.GetMask("Surface","UI"));
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
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.sharedMesh = mapLayout.GenerateCellMesh();
    }
    
    // Occupies and fits a building onto the map
    private void Occupy(BuildingStructure building, Cell[] cells)
    {
        Vector3[][] vertices = new Vector3[cells.Length][];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = CellUnitToWorld(cells[i]);
        }

        mapLayout.Occupy(building, cells);

        building.Fit(vertices, mapLayout.heightFactor);
    }

    // Tries to create and place a building from a world position and rotation, returns if successful
    public bool CreateBuilding(GameObject buildingPrefab, Vector3 worldPosition, int rotation = 0)
    {
        GameObject buildingInstance = Instantiate(buildingPrefab, GameObject.Find("Buildings").transform);
        BuildingStats stats = buildingInstance.GetComponent<BuildingStats>();
        BuildingStructure building = buildingInstance.GetComponent<BuildingStructure>();

        Cell root = GetCell(worldPosition);
        Cell[] cells = GetCells(root, building, rotation);
        
        if (IsValid(cells) && Manager.Spend(stats.ScaledCost))
        {
            mapLayout.Align(cells, rotation);
            Occupy(building, cells);
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
            Gizmos.color = vertexColor;

            foreach (Vertex root in mapLayout.VertexGraph.GetData())
            {
                Gizmos.DrawSphere(root, vertexRadius);
            }

            Gizmos.color = gridColor;
            foreach (Cell cell in mapLayout.CellGraph.GetData())
            {
                cell.DrawCell();
            }

            if (selectingVertex && mapLayout.VertexGraph.Count > 0)
            {
                selectedVertex = Mathf.Clamp(selectedVertex, 0, mapLayout.VertexGraph.Count - 1);

                Gizmos.color = selectedColor;
                Gizmos.DrawSphere(mapLayout.VertexGraph.GetData()[selectedVertex], vertexRadius);

                Gizmos.color = adjacentColor;
                foreach (Vertex adjacent in mapLayout.VertexGraph.GetAdjacent(mapLayout.VertexGraph.GetData()[selectedVertex]))
                    Gizmos.DrawSphere(adjacent, vertexRadius);
            }
            else if (mapLayout.CellGraph.Count > 0)
            {
                selectedCell = Mathf.Clamp(selectedCell, 0, mapLayout.CellGraph.Count - 1);

                Gizmos.color = adjacentColor;
                foreach (Cell adjacent in mapLayout.CellGraph.GetAdjacent(mapLayout.CellGraph.GetData()[selectedCell]))
                    adjacent.DrawCell();

                Gizmos.color = selectedColor;
                mapLayout.CellGraph.GetData()[selectedCell].DrawCell();
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)i / 3);
                    Gizmos.DrawSphere(mapLayout.CellGraph.GetData()[selectedCell].Vertices[i], vertexRadius);
                }
            }

            Gizmos.color = occupiedColor;
            foreach (Cell cell in mapLayout.CellGraph.GetData())
            {
                if (cell.Occupied) cell.DrawCell();
            }
        }
    }
}
