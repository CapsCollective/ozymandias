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

        Cell occupied = mapLayout.Occupy(unitPos);

        BuildingMesh bm = Instantiate(buildingPrefab, transform.TransformPoint(occupied.Centre), Quaternion.identity).GetComponent<BuildingMesh>();
        bm.Fit(
            transform.TransformPoint(occupied.Vertices[0]),
            transform.TransformPoint(occupied.Vertices[1]),
            transform.TransformPoint(occupied.Vertices[2]),
            transform.TransformPoint(occupied.Vertices[3])
            );
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
                if (cell.occupied) cell.DrawCell();
            }
        }
    }
}
