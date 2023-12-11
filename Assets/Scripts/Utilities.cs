using System.Collections.Generic;
using UnityEngine;

public static class Utilities 
{

    /// <summary>
    /// Randomize using Fisher Yates Shuffle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void RandomizeList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; --i)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }
}