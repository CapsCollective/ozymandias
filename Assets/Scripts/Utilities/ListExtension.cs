using System.Collections.Generic;
using System.Linq;
using Structures;
using UnityEngine;

namespace Utilities
{
    public static class ListExtension
    {
        public static void RandomInsert<T>(this List<T> list, T item, int max = 0)
        {
            if (max == 0 || list.Count < 3)
                list.Insert(Random.Range(0, list.Count), item);
            else
                list.Insert(Random.Range(0, max), item);
        }

        public static void Shuffle<T>(this List<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }
    
        public static T PopRandom<T>(this List<T> list)
        {
            int i = Random.Range(0, list.Count);
            T value = list[i];
            list.RemoveAt(i);
            return value;
        }
        
        public static T SelectRandom<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        // Gets a random subset from a list of count
        // Absolutely not the fastest way to do this, fix if needed
        public static List<Blueprint> RandomSelection(this List<Blueprint> list, int count)
        {
            List<Blueprint> dupList = new List<Blueprint>(list);
            for (int i = 0; i < list.Count - count; i++)
            {
                dupList.RemoveAt(Random.Range(0, dupList.Count));
            }

            return dupList;
        }

        public static List<Vector2> Vector3ToVector2(List<Vector3> v3)
        {
            List<Vector2> newList = new List<Vector2>(v3.Count);
            for (int i = 0; i < v3.Count; i++)
            {
                newList[i] = new Vector2(v3[i].x, v3[i].z);
            }
            return newList;
        }
        
        public static bool IsContainedWithin<T>(this List<T> a, Dictionary<T, List<T>> b)
        {
            return b.Values.Any(bT => IsMatch(a, bT));
        }

        public static bool IsMatch<T>(List<T> a, List<T> b)
        {
            return a.All(b.Contains);
        }
    }
}
