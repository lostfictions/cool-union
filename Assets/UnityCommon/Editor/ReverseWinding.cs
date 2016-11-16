using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEditor;

public class ReverseWinding
{
    [MenuItem("Utility/Reverse winding order", true, 15)]
    static bool CanReverseWindingOrder()
    {
        var s = Selection.activeTransform;
        if(s == null) {
            return false;
        }

        var mf = Selection.activeTransform.GetComponent<MeshFilter>();
        if(mf == null) {
            return false;
        }

        return mf.sharedMesh != null;
    }

    [MenuItem("Utility/Reverse winding order", false, 15)]
    static void ReverseWindingOrder()
    {
        MeshFilter mf = Selection.activeTransform.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;

        for(int i = 0; i < triangles.Length; i += 3) {
            int tmp = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = tmp;
        }

        for(int i = 0; i < normals.Length; i++) {
            normals[i] *= -1;
        }

        mesh.triangles = triangles;
        mesh.normals = normals;
    }
}
