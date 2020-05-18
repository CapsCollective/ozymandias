using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapLayout mapLayout;
    public bool selectingVertex;
    public int selectedVertex;
    public int selectedCell;

    [Header("Debug Settings")]
    public bool debug;
    public float vertexRadius;
    public Color selectedColor;
    public Color adjacentColor;
    public Color vertexColor;
    public Color gridColor;
    public Color occupiedColor;

    private MeshFilter _meshFilter;

    public enum HighlightState { Inactive, Valid, Invalid }

    private void Start()
    {
        Generate();
    }

    public void Highlight(Cell[] cells, HighlightState state)
    {
        Vector2[] uv = _meshFilter.sharedMesh.uv;

        foreach (Cell cell in cells)
        {
            foreach (int vertexIndex in mapLayout.TriangleMap[cell])
                uv[vertexIndex].x = (int)state / 2f;
        }

        _meshFilter.sharedMesh.uv = uv;
    }

    public Cell GetClosest(Vector3 worldPosition)
    {
        return mapLayout.GetClosest(transform.InverseTransformPoint(worldPosition));
    }

    public Cell[] GetCells(Cell root, BuildingPlacement.Building building)
    {
        return mapLayout.GetCells(root, building);
    }

    public void Generate()
    {
        mapLayout.Generate(mapLayout.seed);
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.sharedMesh = mapLayout.GenerateCellMesh();
    }

    public void Occupy(GameObject prefab, Vector3 worldPosition)
    {
        BuildingPlacement.Building building = Instantiate(prefab).GetComponent<BuildingPlacement.Building>();

        // Convert world to local position
        Vector3 unitPosition = transform.InverseTransformPoint(worldPosition);

        Cell[] cells = mapLayout.GetCells(building, unitPosition);

        if (cells != null)
        {
            Vector3[][] vertices = new Vector3[cells.Length][];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = CellUnitToWorld(cells[i]);
                cells[i].Occupy(building);
            }

            building.Fit(vertices);
        }
        else
        {
            Destroy(building.gameObject);
        }
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
