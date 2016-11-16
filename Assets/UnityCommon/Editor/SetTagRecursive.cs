using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEditor;

public class SetTagRecursive
{
    [MenuItem("Utility/Assign Current Tag to Children", true, 500)]
    public static bool CanSetTagRecursively()
    {
        return Selection.activeTransform != null && Selection.activeTransform.childCount > 0;
    }

    [MenuItem("Utility/Assign Current Tag to Children", false, 500)]
    public static void SetTagRecursively()
    {
        SetTagRecursively(Selection.activeTransform);
    }

    static void SetTagRecursively(Transform t)
    {
        string tag = t.tag;
        foreach(Transform c in t) {
            c.tag = tag;
            if(c.childCount > 0) {
                SetTagRecursively(c);
            }
        }
    }
}
