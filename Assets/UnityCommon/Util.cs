using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Util
{
    public static int EnumCount<T>()
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    public static string EnumString(Enum e)
    {
        return Enum.GetName(e.GetType(), e);
    }

    public static T AddOrGetComponent<T>(this GameObject go) where T : Component
    {
        //We'd like to do this, but the Unity "fake null" stuff causes problems...
        //return go.GetComponent<T>() ?? go.AddComponent<T>();

        T c = go.GetComponent<T>();
        if(!c) {
			c = go.AddComponent<T>();
		}
		return c;
    }
}
