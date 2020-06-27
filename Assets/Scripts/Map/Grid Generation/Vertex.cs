using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    private Vector3 position;

    public bool Split { get; }
    public bool Boundary { get; }

    public Vertex(Vector3 _position, bool _split, bool _boundary)
    {
        position = _position;
        Split = _split;
        Boundary = _boundary;
    }

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }

    public static implicit operator Vector3(Vertex vertex)
    {
        return vertex.position;
    }

    public static bool operator ==(Vertex first, Vertex second)
    {
        return first.GetHashCode() == second.GetHashCode() && (Vector3)first == second;
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