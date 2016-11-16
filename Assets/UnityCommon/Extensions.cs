using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Collections;
using JetBrains.Annotations;

public static class Extensions
{
    [Pure]
    public static int IncrementAndWrap(this IList t, int currentIndex)
    {
        int newIndex = currentIndex + 1;
        return (newIndex >= t.Count) ? 0 : newIndex;
    }

    [Pure]
    public static int DecrementAndWrap(this IList t, int currentIndex)
    {
        int newIndex = currentIndex - 1;
        return (newIndex < 0) ? t.Count - 1 : newIndex;
    }

    [Pure]
    public static T RandomInRange<T>(this IList<T> t)
    {
        if(t.Count == 0) {
            Debug.LogError("Cannot return random value: list is empty!");
            return default(T);
        }
        return t[Random.Range(0, t.Count)];
    }

    [Pure]
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if(val.CompareTo(max) > 0) {
            return max;
        }
        if(val.CompareTo(min) < 0) {
            return min;
        }
        return val;
    }

    /// Retrieves a substring from this instance. The string starts at [startIndex]
    /// and ends at [endIndex - 1].
    [Pure]
    public static string Substr(this string s, int startIndex, int endIndex)
    {
        return s.Substring(startIndex, endIndex - startIndex);
    }

    [Pure]
    public static Color Invert(this Color c)
    {
        return new Color(1f - c.r, 1f - c.g, 1f - c.b, c.a);
    }
}
