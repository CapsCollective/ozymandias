using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Map
{
    // Store the id returned on Add within T and use for all querying
    [Serializable]
    public class Graph<T>
    {
        [field: SerializeField] private SerializedDictionary<int, T> DataMap { get; set; }
        [field: SerializeField] private SerializedDictionary<int, List<int>> AdjacencyMap { get; set; }

        private int _nextId;
        
        public int Count => DataMap.Count;

        public List<int> Ids => DataMap.Keys.ToList();
        public List<T> Data => DataMap.Values.ToList();

        public Graph()
        {
            _nextId = 0;
            DataMap = new SerializedDictionary<int, T>();
            AdjacencyMap = new SerializedDictionary<int, List<int>>();
        }

        public Graph(Graph<T> oldGraph)
        {
            DataMap = new SerializedDictionary<int, T>();
            AdjacencyMap = new SerializedDictionary<int, List<int>>();
        
            foreach (KeyValuePair<int, T> data in oldGraph.DataMap)
            {
                DataMap.Add(data.Key, data.Value);
                AdjacencyMap.Add(data.Key, new List<int>(oldGraph.AdjacencyMap[data.Key]));
                if (_nextId <= data.Key) _nextId = data.Key + 1;
            }
        }

        public T GetData(int id)
        {
            return DataMap[id];
        }
        
        public int Add(T toAdd, int ownId = -1)
        {
            bool useOwnId = ownId != -1;
            int id = useOwnId ? ownId : _nextId++;
            DataMap.Add(id, toAdd);
            AdjacencyMap.Add(id, new List<int>());
            return id;
        }

        public int Add(T toAdd, List<int> adjacencyMap, int ownId = -1)
        {
            bool useOwnId = ownId != -1;
            int id = useOwnId ? ownId : _nextId++;
            DataMap.Add(id, toAdd);
            AdjacencyMap.Add(id, adjacencyMap);
            return id;
        }

        public void Remove(int id)
        {
            foreach (int id2 in AdjacencyMap[id])
            {
                RemoveEdge(id2, id, false);
            }
            DataMap.Remove(id);
            AdjacencyMap.Remove(id);
        }

        public void AddEdge(int idA, int idB, bool bidirectional = true)
        {
            // Checks for validity (both points exist) and adds either in one or both directions
            if (!AdjacencyMap.ContainsKey(idA)) Debug.LogError("ID not found: " + idA);
            AdjacencyMap[idA].Add(idB);
            if (bidirectional) AddEdge(idB, idA, false);
        }

        public void RemoveEdge(int idA, int idB, bool bidirectional = true)
        {
            if (!AdjacencyMap.ContainsKey(idA)) return;
            AdjacencyMap[idA]?.Remove(idB);
            if (bidirectional) RemoveEdge(idB, idA, false);
        }
        
        public void RemoveAllEdges()
        {
            foreach (int key in AdjacencyMap.Keys)
                AdjacencyMap[key].Clear();
        }

        public bool Contains(int id)
        {
            return DataMap.ContainsKey(id);
        }

        public bool Contains(T data)
        {
            return DataMap.Values.Contains(data);
        }
        
        public bool HasSharedNeighbours(int idA, int idB)
        {
            // Compares all neighbours of idA to idB
            return (from n1 in AdjacencyMap[idA] from n2 in AdjacencyMap[idB] where n1.Equals(n2) select n1).Any();
        }

        public List<T> SharedNeighbours(int idA, int idB)
        {
            // Compares all neighbours of idA to idB and gets the data of matches
            return (from n1 in AdjacencyMap[idA] from n2 in AdjacencyMap[idB] where n1 == n2 select DataMap[n1]).ToList();
        }
        
        public bool IsAdjacent(int idA, int idB)
        {
            return AdjacencyMap.ContainsKey(idA) && AdjacencyMap[idA].Contains(idB);
        }

        public List<int> GetAdjacent(int id)
        {
            if (AdjacencyMap.ContainsKey(id)) return AdjacencyMap[id];
            throw new Exception("Node queried is not contained in the adjacency map.");
        }

        public List<T> GetAdjacentData(int id)
        {
            return GetAdjacent(id).Select(x => DataMap[x]).ToList();
        }

        // Recursive depth first search
        public List<List<int>> Search(int start, int end, int limit, bool direct)
        {
            List<List<int>> paths = new List<List<int>>();
            Stack<int> stack = new Stack<int>();
            Visit(start, start);
            return paths;

            void Visit(int current, int previous)
            {
                stack.Push(current);

                if (current == end && stack.Count > 1)
                {
                    List<int> currentPath = new List<int>(stack);
                    paths.Add(currentPath);
                }
                else if (stack.Count < limit)
                {
                    // Visit all valid neighbours
                    foreach (int neighbour in AdjacencyMap[current].Where(neighbour => 
                        (!direct && !(neighbour.Equals(end) && current.Equals(start)) && !stack.Contains(neighbour)) || 
                        (direct && !neighbour.Equals(previous))
                    )) {
                        Visit(neighbour, current);
                    }
                }

                stack.Pop();
            }
        }
    }
}
