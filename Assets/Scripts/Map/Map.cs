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

    //private Matrix4x4 UnitToWorldMatrix { get { return Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale); } }

    private void OnDrawGizmos()
    {
        if (mapLayout && debug)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = vertexColor;

            foreach (Vertex root in mapLayout.Vertices)
            {
                Gizmos.DrawSphere(root, vertexRadius);
            }

            Gizmos.color = gridColor;
            foreach (Cell cell in mapLayout.Cells)
            {
                cell.DrawCellGizmo();
            }

            if (selectingVertex && mapLayout.Vertices.Count > 0)
            {
                selectedVertex = Mathf.Clamp(selectedVertex, 0, mapLayout.Vertices.Count - 1);

                Gizmos.color = selectedColor;
                Gizmos.DrawSphere(mapLayout.Vertices[selectedVertex], vertexRadius);

                Gizmos.color = adjacentColor;
                foreach (Vertex adjacent in mapLayout.GetAdjacent(mapLayout.Vertices[selectedVertex]))
                    Gizmos.DrawSphere(adjacent, vertexRadius);
            }
            else if (mapLayout.Cells.Count > 0)
            {
                selectedCell = Mathf.Clamp(selectedCell, 0, mapLayout.Cells.Count - 1);

                Gizmos.color = adjacentColor;
                foreach (Cell adjacent in mapLayout.GetAdjacent(mapLayout.Cells[selectedCell]))
                    adjacent.DrawCellGizmo();

                Gizmos.color = selectedColor;
                mapLayout.Cells[selectedCell].DrawCellGizmo();
            }
        }
    }
}
