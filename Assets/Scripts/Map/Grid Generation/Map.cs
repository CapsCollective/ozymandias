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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Occupy(clickPos);
        }
    }

    public void Occupy(Vector3 worldPos)
    {
        Vector3 unitPos = transform.InverseTransformPoint(worldPos);
        mapLayout.Occupy(unitPos);
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
            }

            Gizmos.color = occupiedColor;
            foreach (Cell cell in mapLayout.CellGraph.GetData())
            {
                if (cell.occupied) cell.DrawCell();
            }
        }
    }
}
