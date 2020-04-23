using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Layout", menuName = "Map Layout", order = 50)]
public class MapLayout : ScriptableObject
{
    public List<Vertex> Vertices { get; private set; } = new List<Vertex>();
    public List<Cell> Cells { get; private set; } = new List<Cell>();

    //private readonly Dictionary<Vertex, List<Vertex>> VertexMap = new Dictionary<Vertex, List<Vertex>>();
    //private readonly Dictionary<Cell, List<Cell>> CellMap = new Dictionary<Cell, List<Cell>>();

    //public bool IsAdjacent(Vertex root, Vertex neighbour) { return VertexMap.ContainsKey(root) && VertexMap[root].Contains(neighbour); }

    //public bool IsAdjacent(Cell root, Cell neighbour) { return CellMap.ContainsKey(root) && CellMap[root].Contains(neighbour); }
}

public class Vertex
{
    public Vector3 position;
    public readonly List<Vertex> Adjacent = new List<Vertex>();

    public Vertex(Vector3 _position)
    {
        position = _position;
    }

    public static implicit operator Vector3(Vertex x)
    {
        return x.position;
    }

    public bool IsAdjacent(Vertex other) { return Adjacent.Contains(other); }
}

public class Cell
{
    public Vertex[] Vertices { get; private set; }
    public readonly List<Cell> Adjacent = new List<Cell>();

    public Cell(Vertex vertexA, Vertex vertexB, Vertex vertexC, Vertex vertexD)
    {
        Vertices = new Vertex[] { vertexA, vertexB, vertexC, vertexD };
    }

    public Cell(Vertex[] _Vertices)
    {
        if (_Vertices.Length != 4) throw new System.Exception("Incorrect number of vertices used to create new cell: " + _Vertices.Length + " - should be 4.");
        else Vertices = _Vertices;
    }

    public bool IsAdjacent(Cell other) { return Adjacent.Contains(other); }
}
