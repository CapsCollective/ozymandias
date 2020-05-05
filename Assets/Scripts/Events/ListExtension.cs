using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static void RandomInsert<T>(this List<T> list, T item)
    {
        list.Insert(Random.Range(0, list.Count), item);
    }

    public static void Shuffle<T>(this List<T> list)
    {

    }
}
