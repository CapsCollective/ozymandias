using System.Collections.Generic;
using System.Linq;

namespace Entities
{
    public class Graph<T>
    {
        public List<T> Data { get; }
        private Dictionary<T, List<T>> Map { get; }

        public int Count => Data.Count;

        public Graph()
        {
            Data = new List<T>();
            Map = new Dictionary<T, List<T>>();
        }

        public Graph(Graph<T> oldGraph)
        {
            Data = new List<T>(oldGraph.Data);
            Map = new Dictionary<T, List<T>>();
        
            foreach (T key in oldGraph.Map.Keys)
            {
                Map.Add(key, new List<T>(oldGraph.Map[key]));
            }
        }

        public Graph(Graph<T> oldGraph, List<T> toInclude)
        {
            Data = new List<T>();
            Data.AddRange(toInclude.Where(include => !Data.Contains(include)));
            Map = new Dictionary<T, List<T>>();

            foreach (T include in toInclude)
            {
                if (!oldGraph.Contains(include)) continue;
                foreach (T neighbour in oldGraph.GetAdjacent(include).Where(Contains))
                    CreateEdge(include, neighbour);
            }
        }

        public bool Contains(T toCheck)
        {
            return Data.Contains(toCheck);
        }

        public void RemoveEdges()
        {
            foreach (T key in Map.Keys)
                Map[key].Clear();
        }

        public List<List<T>> OneWayDFS(T root, T target, int limit)
        {
            List<List<T>> paths = new List<List<T>>();

            Stack<T> stack = new Stack<T>();

            OneWayVisit(root, root, target, ref paths, ref stack, limit);

            return paths;
        }

        private void OneWayVisit(T current, T previous, T target, ref List<List<T>> paths, ref Stack<T> stack, int limit)
        {
            stack.Push(current);

            if (current.Equals(target) && stack.Count > 1)
            {
                List<T> currentPath = new List<T>(stack);
                paths.Add(currentPath);
            }
            else if (stack.Count < limit)
            {
                foreach (T neighbour in Map[current])
                {
                    if (!neighbour.Equals(previous)) OneWayVisit(neighbour, current, target, ref paths, ref stack, limit);
                }
            }

            stack.Pop();
        }

        public List<List<T>> IndirectDFS(T root, T dest, int limit)
        {
            List<List<T>> paths = new List<List<T>>();

            Stack<T> stack = new Stack<T>();

            IndirectVisit(root, root, dest, ref paths, ref stack, limit);

            return paths;
        }

        private void IndirectVisit(T root, T current, T dest, ref List<List<T>> paths, ref Stack<T> stack, int limit)
        {
            stack.Push(current);

            if (current.Equals(dest) && stack.Count > 1)
            {
                List<T> currentPath = new List<T>(stack);
                paths.Add(currentPath);
            }
            else if (stack.Count < limit)
            {
                foreach (T neighbour in Map[current])
                {
                    if (!(neighbour.Equals(dest) && current.Equals(root)) && !stack.Contains(neighbour))
                        IndirectVisit(root, neighbour, dest, ref paths, ref stack, limit);
                }
            }

            stack.Pop();
        }

        public bool HasSharedNeighbours(T v1, T v2)
        {
            foreach (T n1 in Map[v1])
            {
                foreach (T n2 in Map[v2])
                {
                    if (n1.Equals(n2)) return true;
                }
            }
            return false;
        }

        public List<T> SharedNeighbours(T v1, T v2)
        {
            List<T> shared = new List<T>();
            foreach (T n1 in Map[v1])
            foreach (T n2 in Map[v2])
                if (n1.Equals(n2))
                    shared.Add(n1);
            return shared;
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
}
