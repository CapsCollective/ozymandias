using System;
using System.Collections.Generic;
using System.Linq;
using Structures;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class Cell
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public bool Active { get; set; }
        [field: SerializeField] public List<Vertex> Vertices { get; set; }
        public Structure Occupant { get; set; }
        public int Rotation { get; set; }
        
        public Vector3 Centre => (Vertices[0] + Vertices[1] + Vertices[2] + Vertices[3]) / 4;
        public Vector3 WorldSpace => Quaternion.Euler(90f, 0f, 30f) * Centre;
        public bool Occupied => Occupant;
        
        public Cell(List<Vertex> vertices, bool active = true, bool safe = false)
        {
            vertices = vertices.GetRange(0, 4); // Limit to 4 points
            Active = active;
            // Ensure cell is convex and correctly oriented
            if(Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).z > 0) vertices.Reverse();
            Vertices = vertices;
        }
        
        public static bool IsValid(List<Cell> cells)
        {
            return cells.All(IsValid) && cells.Distinct().Count() == cells.Count;
        }

        public static bool IsValid(Cell cell)
        {
            return cell != null && !cell.Occupied && cell.Active;
        }

        public static bool operator ==(Cell cell, Cell other)
        {
            if (ReferenceEquals(other, null))
            {
                return ReferenceEquals(cell, null);
            }
            if (cell == null) return false;
            
            foreach (Vertex vertex in cell.Vertices)
            {
                bool contains = false;

                foreach (Vertex otherVertex in other.Vertices)
                {
                    if (vertex == otherVertex)
                        contains = true;
                }

                if (!contains) return false;
            }

            return true;
        }

        public static bool operator !=(Cell cell, Cell other)
        {
            return !(cell == other);
        }
        
        public override bool Equals(object obj)
        {
            return this == (Cell)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
