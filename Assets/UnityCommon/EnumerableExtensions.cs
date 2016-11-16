using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Linq;
using JetBrains.Annotations;

public static partial class EnumerableExtensions
{
    //From https://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm
    [Pure]
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        T[] elements = source.ToArray();
        // Note i > 0 to avoid final pointless iteration
        for(int i = elements.Length - 1; i > 0; i--) {
            // Swap element "i" with a random earlier element it (or itself)
            int swapIndex = Random.Range(0, i + 1);
            yield return elements[swapIndex];
            elements[swapIndex] = elements[i];
            // we don't actually perform the swap, we can forget about the
            // swapped element because we already returned it.
        }

        // there is one item remaining that was not returned - we return it now
        yield return elements[0];
    }

    //From http://stackoverflow.com/a/24016130
    [Pure]
    public static IEnumerable<int> IndexOfAll(this string s, char c)
    {
        int startIndex = s.IndexOf(c);
        while(startIndex != -1) {
            yield return startIndex;
            startIndex = s.IndexOf(c, startIndex + 1);
        }
    }

    [Pure]
    public static string Log<T>(this IEnumerable<T> ie, string delimiter)
    {
        return ie.FirstOrDefault() + ie.Skip(1).Aggregate("", (s, i) => s + delimiter + i);
    }

    // Handy utility functions for iteration with or without index, from
    // http://stackoverflow.com/questions/521687/c-sharp-foreach-with-index
    public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
    {
        int i = 0;
        foreach(var e in ie) {
            action(e, i++);
        }
    }

    public static void Each<T>(this IEnumerable<T> ie, Action<T> action)
    {
        foreach(var e in ie) {
            action(e);
        }
    }
}
