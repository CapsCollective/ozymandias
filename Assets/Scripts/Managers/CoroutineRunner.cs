using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;
using System;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner _Instance;

    private void Awake()
    {
        if(_Instance == null)
            _Instance = this;
    }

    public IEnumerator ConvexHullAsync(List<Vertex> included, Graph<Vertex> VertexGraph, Action<List<Vertex>> returnThing)
    {
        List<Vertex> path = new List<Vertex>();

        // Ensure that the first is on the path
        Vertex p = included[0];
        for (int i = 1; i < included.Count; i++)
            if (((Vector3)included[i]).x < ((Vector3)p).x)
                p = included[i];

        path.Add(p);

        while (path.Count == 1 || path[path.Count - 1] != path[0])
        {
            // Initialise the set of possible points
            List<Vertex> s = new List<Vertex>(included);

            Vertex q = s[0];
            foreach (Vertex n in s)
            {
                if (n == p)
                    continue;
                bool isNext = true;
                foreach (Vertex r in s)
                {
                    if (r != p && Vector3.Cross(p - n, r - n).z > 0)
                        isNext = false;
                }
                if (isNext) q = n;
            }

            path.Add(q);
            p = q;
        }

        Dictionary<Vertex, List<Vertex>> toInclude = new Dictionary<Vertex, List<Vertex>>();

        Graph<Vertex> dupGraph = new Graph<Vertex>(VertexGraph);
        for (int i = dupGraph.Count - 1; i >= 0; i--)
            if (!included.Contains(dupGraph.GetData()[i]))
                dupGraph.RemoveAt(i);

        for (int i = 0; i < path.Count - 1; i++)
        {
            toInclude.Add(
                path[i],
                AStar(dupGraph, path[i], path[(i + 1) % path.Count], 2000)
                );
        }

        foreach (Vertex key in toInclude.Keys)
        {
            path.InsertRange(
                (path.IndexOf(key) + 1) % path.Count,
                toInclude[key].GetRange(1, toInclude[key].Count - 2)
                );
        }

        returnThing?.Invoke(path);
        yield return new WaitForEndOfFrame();
    }

    public List<Vertex> AStar(Graph<Vertex> set, Vertex root, Vertex target, int iterationLimit = 2000)
    {
        List<Vertex> path = new List<Vertex>();

        if (root == target)
            return path;

        List<Vertex> open = new List<Vertex>() { root };
        Dictionary<Vertex, Costs> costs = new Dictionary<Vertex, Costs>();
        List<Vertex> closed = new List<Vertex>();

        for (int iteration = 0; iteration < iterationLimit; iteration++)
        {
            Vertex current = open[0];
            for (int i = 1; i < open.Count; i++)
                if (costs[open[i]].FCost < costs[current].FCost) current = open[i];

            open.Remove(current);
            closed.Add(current);

            if (current.Equals(target))
            {
                path.Add(root);
                path.Add(target);

                Vertex parent = costs[current].Parent;
                while (parent != root)
                {
                    path.Insert(1, parent);
                    parent = costs[parent].Parent;
                }
                break;
            }

            foreach (Vertex neighbour in set.GetAdjacent(current))
            {
                float GCost = Vector3.Distance(neighbour, root);
                float HCost = Vector3.Distance(neighbour, target);
                float FCost = GCost + HCost;

                if (closed.Contains(neighbour))
                    continue;

                if ((costs.ContainsKey(neighbour) && costs[neighbour].FCost >= FCost) || !open.Contains(neighbour))
                {
                    if (costs.ContainsKey(neighbour))
                        costs.Remove(neighbour);
                    costs.Add(neighbour, new Costs(current, GCost, HCost, FCost));

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }

        return path;
    }

    private struct Costs
    {
        public float GCost { get; }
        public float HCost { get; }
        public float FCost { get; }
        public Vertex Parent { get; }

        public Costs(Vertex _Parent, float _GCost, float _HCost, float _FCost)
        {
            Parent = _Parent;
            GCost = _GCost;
            HCost = _HCost;
            FCost = _FCost;
        }
    }
}
