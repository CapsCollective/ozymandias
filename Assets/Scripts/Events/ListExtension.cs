using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
