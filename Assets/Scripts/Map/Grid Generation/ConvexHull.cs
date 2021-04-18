/* This is taken from this blog post:
 * http://loyc-etc.blogspot.ca/2014/05/2d-convex-hull-in-c-45-lines-of-code.html
 *
 * All I have done is renamed "DList" to "CircularList" and then wrote a wrapper for the generic C# list.
 * The structure that is supposed to be used is *much* more efficient, but this works for my purposes.
 *
 * This can be dropped right into your Unity project and will work without any adjustments.
 */
using System.Collections.Generic;
using UnityEngine;

public static class ConvexHull
{
    public static IList<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false)
    {
        if (!sortInPlace)
            points = new List<Vector2>(points);
        points.Sort((a, b) =>
            a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : -1));

        // Importantly, DList provides O(1) insertion at beginning and end
        CircularList<Vector2> hull = new CircularList<Vector2>();
        int L = 0, U = 0; // size of lower and upper hulls

        // Builds a hull such that the output polygon starts at the leftmost Vector2.
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector2 p = points[i], p1;

            // build lower hull (at end of output list)
            while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
            {
                hull.PopLast();
                L--;
            }
            hull.PushLast(p);
            L++;

            // build upper hull (at beginning of output list)
            while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
            {
                hull.PopFirst();
                U--;
            }
            if (U != 0) // when U=0, share the Vector2 added above
                hull.PushFirst(p);
            U++;
            Debug.Assert(U + L == hull.Count + 1);
        }
        hull.PopLast();
        return hull;
    }

    private static Vector2 Sub(this Vector2 a, Vector2 b)
    {
        return a - b;
    }

    private static float Cross(this Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    private class CircularList<T> : List<T>
    {
        public T Last
        {
            get
            {
                return this[this.Count - 1];
            }
            set
            {
                this[this.Count - 1] = value;
            }
        }

        public T First
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        public void PushLast(T obj)
        {
            this.Add(obj);
        }

        public T PopLast()
        {
            T retVal = this[this.Count - 1];
            this.RemoveAt(this.Count - 1);
            return retVal;
        }

        public void PushFirst(T obj)
        {
            this.Insert(0, obj);
        }

        public T PopFirst()
        {
            T retVal = this[0];
            this.RemoveAt(0);
            return retVal;
        }
    }
}