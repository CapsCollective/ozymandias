using System;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class Vertex
    { 
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public bool Split { get; private set; }
        [field: SerializeField] public bool Boundary { get; private set; }

        public Vertex(Vector3 position, bool split, bool boundary)
        {
            Position = position;
            Split = split;
            Boundary = boundary;
        }

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public static implicit operator Vector3(Vertex vertex)
        {
            return vertex.Position;
        }

        public static bool operator ==(Vertex first, Vertex second)
        {
            if ((object) first == null)
                return (object) second == null;
            return second != null && (first.Id == second.Id && (Vector3)first == second);
        }

        public static bool operator !=(Vertex first, Vertex second)
        {
            return !(first == second);
        }

        public static Vector3 operator +(Vertex first, Vertex second)
        {
            return (Vector3)first + second;
        }

        public static Vector3 operator -(Vertex first, Vertex second)
        {
            return (Vector3)first - second;
        }
        
        protected bool Equals(Vertex other)
        {
            return Id == other.Id && Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Vertex) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ Position.GetHashCode();
            }
        }
    }
}
