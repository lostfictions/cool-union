using UnityEngine;
using UnityEditor;

public static class FindSceneObjects
{
    [MenuItem("Utility/Find Missing Scripts")]
    static void FindMissingScripts()
    {
        var transforms = Object.FindObjectsOfType<Transform>();

        foreach(var t in transforms) {
            var components = t.GetComponents<Component>();
            foreach(var c in components) {
                if(c == null) {
                    Debug.Log(TransformHierarchyTools.GetTransformPath(t) + " has a missing script!");

                    if(t.hideFlags != 0) {
                        t.gameObject.hideFlags = 0;
                        Debug.Log("IT HAS BEEN REVEALED.");
                    }
                }
            }
        }
    }

    [MenuItem("Utility/Find Hidden Scene Objects")]
    static void FindHiddenSceneObjects()
    {
        var transforms = Object.FindObjectsOfType<Transform>();

        foreach(var t in transforms) {
            if(t.hideFlags != 0) {
                string log = TransformHierarchyTools.GetTransformPath(t) + " has hide flags: ";
                if((t.hideFlags & HideFlags.DontSave) != 0) {
                    log += "DontSave ";
                }
                if((t.hideFlags & HideFlags.HideInHierarchy) != 0) {
                    log += "HideInHierarchy ";
                }
                if((t.hideFlags & HideFlags.HideAndDontSave) != 0) {
                    log += "HideAndDontSave ";
                }
                if((t.hideFlags & HideFlags.HideInInspector) != 0) {
                    log += "HideInInspector ";
                }
                if((t.hideFlags & HideFlags.NotEditable) != 0) {
                    log += "NotEditable ";
                }

                Debug.Log(log);
            }
        }
    }
}
