using UnityEngine;

namespace Entities
{
    public class Vertex
    {
        private Vector3 Position { get; set; }

        public bool Split { get; }
        public bool Boundary { get; }

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
            return second != null && (first.GetHashCode() == second.GetHashCode() && (Vector3)first == second);
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

        public override bool Equals(object obj)
        {
            return this == (Vertex)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
