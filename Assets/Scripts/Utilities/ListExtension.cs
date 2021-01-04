using System.Collections.Generic;
using System.Linq;
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
    }
}
