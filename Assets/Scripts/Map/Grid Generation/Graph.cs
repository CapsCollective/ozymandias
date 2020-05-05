using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph<T>
{
    private readonly List<T> Data = new List<T>();
    private readonly Dictionary<T, List<T>> Map = new Dictionary<T, List<T>>();

    public int Count { get { return Data.Count; } }

    public Graph()
    {
        Data = new List<T>();
        Map = new Dictionary<T, List<T>>();
    }

    public Graph(Graph<T> oldGraph)
    {
        Data = new List<T>(oldGraph.Data);
        Map = new Dictionary<T, List<T>>(oldGraph.Map);
    }

    public List<T> GetData()
    {
        return Data;
    }

    public bool Contains(T toCheck)
    {
        return Data.Contains(toCheck);
    }

    public void Add(T toAdd)
    {
        Data.Add(toAdd);
        Map.Add(toAdd, new List<T>());
    }

    public void Remove(T toRemove)
    {
        for (int i = Map[toRemove].Count - 1; i >= 0; i--)
        {
            DestroyEdge(toRemove, Map[toRemove][i]);
        }
        Map.Remove(toRemove);

        Data.Remove(toRemove);
    }

    public void RemoveAt(int index)
    {
        Remove(Data[index]);
    }

    public bool IsAdjacent(T root, T other)
    {
        return Map.ContainsKey(root) && Map[root].Contains(other);
    }

    public List<T> GetAdjacent(T root)
    {
        if (Map.ContainsKey(root))
            return Map[root];
        else
            throw new System.Exception("Vertex queried is not contained in the adjacency map.");
    }

    public void CreateEdge(T vertexA, T vertexB)
    {
        CreateDirectedEdge(vertexA, vertexB);
        CreateDirectedEdge(vertexB, vertexA);
    }

    public void CreateEdge(int indexA, int indexB)
    {
        CreateDirectedEdge(Data[indexA], Data[indexB]);
        CreateDirectedEdge(Data[indexB], Data[indexA]);
    }

    public void DestroyEdge(T vertexA, T vertexB)
    {
        DestroyDirectedEdge(vertexA, vertexB);
        DestroyDirectedEdge(vertexB, vertexA);
    }

    public void DestroyEdge(int indexA, int indexB)
    {
        DestroyDirectedEdge(Data[indexA], Data[indexB]);
        DestroyDirectedEdge(Data[indexB], Data[indexA]);
    }

    private void CreateDirectedEdge(T from, T to)
    {
        if (Map.ContainsKey(from) && !Map[from].Contains(to))
            Map[from].Add(to);
    }

    private void DestroyDirectedEdge(T from, T to)
    {
        if (Map.ContainsKey(from) && Map[from].Contains(to))
            Map[from].Remove(to);
    }
}