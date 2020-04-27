﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapLayout mapLayout;

    [Header("Debug Settings")]
    public bool debug;
    public float vertexRadius;
    public Color vertexColor;
    public Color gridColor;

    private Matrix4x4 UnitToWorldMatrix { get { return Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale); } }

    private void OnDrawGizmos()
    {
        if (mapLayout && debug)
        {
            Gizmos.matrix = UnitToWorldMatrix;

            foreach (Vertex root in mapLayout.Vertices)
            {
                Gizmos.color = vertexColor;
                Gizmos.DrawSphere(root, vertexRadius);
                Gizmos.color = gridColor;

                foreach (Vertex neighbour in mapLayout.GetAdjacent(root))
                {
                    Gizmos.DrawLine(root, neighbour);
                }
            }

            //foreach (Vertex root in mapLayout.Vertices)
            //{
            //    Gizmos.color = vertexColor;
            //    Gizmos.DrawSphere(root, vertexRadius);
            //    Gizmos.color = gridColor;

            //    foreach (Vertex neighbour in root.Adjacent)
            //    {
            //        //Gizmos.DrawLine(root, neighbour);
            //    }
            //}
        }
    }
}
