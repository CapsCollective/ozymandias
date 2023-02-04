using System;
using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;

namespace Utilities
{
    public class Algorithms : MonoBehaviour
    {
        public static void ConvexHullAsync(List<Vertex> included, Graph<Vertex> graph, Action<List<Vertex>> callback)
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
                    if (n == p) continue;
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

            Graph<Vertex> dupGraph = new Graph<Vertex>();
            foreach (var item in included)
            {
                List<int> adjacents = graph.GetAdjacent(item.Id);
                List<int> newAdjacents = new List<int>();
                foreach (var a in adjacents)
                {
                    foreach (var v in included)
                    {
                        if (a == v.Id) newAdjacents.Add(a);
                    }
                }
                dupGraph.Add(graph.GetData(item.Id), newAdjacents, item.Id);
            }



            for (int i = 0; i < path.Count - 1; i++)
            {
                toInclude.Add(
                    path[i],
                    AStar(dupGraph, path[i], path[(i + 1) % path.Count])
                );
            }

            foreach (Vertex key in toInclude.Keys)
            {
                path.InsertRange(
                    (path.IndexOf(key) + 1) % path.Count,
                    toInclude[key].GetRange(1, toInclude[key].Count - 2)
                );
            }

            callback?.Invoke(path);
            //yield return new WaitForEndOfFrame();
        }

        public static List<Vertex> AStar(Graph<Vertex> set, Vertex root, Vertex target, int iterationLimit = 2000)
        {
            List<Vertex> path = new List<Vertex>();
            
            if (root == null || target == null || root == target)
                return path;

            List<Vertex> open = new List<Vertex> { root };
            Dictionary<int, Costs> costs = new Dictionary<int, Costs>();
            List<Vertex> closed = new List<Vertex>();

            for (int iteration = 0; iteration < iterationLimit; iteration++)
            {
                Vertex current = open[0];
                for (int i = 1; i < open.Count; i++)
                    if (costs[open[i].Id].FCost < costs[current.Id].FCost) current = open[i];

                open.Remove(current);
                closed.Add(current);

                if (current.Id == target.Id)
                {
                    path.Add(root);
                    path.Add(target);

                    Vertex parent = costs[current.Id].Parent;
                    while (parent.Id != root.Id)
                    {
                        path.Insert(1, parent);
                        parent = costs[parent.Id].Parent;
                    }
                    break;
                }

                foreach (int neighbourID in set.GetAdjacent(current.Id))
                {
                    Vertex neighbour = set.GetData(neighbourID);
                    if (!neighbour.Active) continue;

                    float gCost = Vector3.Distance(neighbour, root);
                    float hCost = Vector3.Distance(neighbour, target);
                    float fCost = gCost + hCost;

                    if (
                        closed.Contains(neighbour) || 
                        open.Contains(neighbour) && (
                            !costs.ContainsKey(neighbour.Id) || !(costs[neighbour.Id].FCost >= fCost)
                        )
                    ) continue;

                    if (costs.ContainsKey(neighbour.Id))
                        costs.Remove(neighbour.Id);
                    costs.Add(neighbour.Id, new Costs(current, fCost));

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
            return path;
        }

        public static IEnumerator DelayCall(float duration, Action callback)
        {
            // Defer callback action by duration
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }

        private readonly struct Costs
        {
            public float FCost { get; }
            public Vertex Parent { get; }

            public Costs(Vertex parent, float fCost)
            {
                Parent = parent;
                FCost = fCost;
            }
        }
    }
}
