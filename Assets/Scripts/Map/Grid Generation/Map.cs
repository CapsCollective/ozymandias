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

    private void Start()
    {
        mapLayout.Generate(mapLayout.seed);
        GetComponent<MeshFilter>().sharedMesh = mapLayout.GenerateMesh();
    }

    public void Occupy(GameObject buildingPrefab, Vector3 worldPos)
    {
        Vector3 unitPos = transform.InverseTransformPoint(worldPos);
        Cell[] targets = mapLayout.GetClosestUnoccupied(unitPos, buildingPrefab.GetComponent<BuildingMesh>());
        
        if (targets != null)
        {
            BuildingMesh building = Instantiate(buildingPrefab, transform.TransformPoint(targets[0].Centre), Quaternion.identity).GetComponent<BuildingMesh>();
            building.GetComponent<Building>().Build();
            BuildingMesh bm = building.GetComponent<BuildingMesh>();

            Vector3[][] vertices = new Vector3[targets.Length][];
            for (int i = 0; i < targets.Length; i++)
                vertices[i] = CellUnitToWorld(targets[i]);

            bm.Fit(vertices);

            foreach (Cell cell in targets)
                cell.Occupy(bm);
        }
    }

    private Vector3[] CellUnitToWorld(Cell cell)
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
