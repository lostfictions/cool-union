using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//Based on Pixelplacement's original script "Group"
public class Grouping
{
    [MenuItem("Utility/Group Selected %g", true, 1)]
    static bool CanGroupSelected()
    {
        return Selection.transforms.Length > 0;
    }

    [MenuItem("Utility/Ungroup Selected %#g", true, 2)]
    static bool CanUngroupSelected()
    {
        return Selection.activeTransform != null && Selection.activeTransform.childCount > 0;
    }

    [MenuItem("Utility/Group Selected %g", false, 1)]
    static void GroupSelected()
    {
        //Cache selected objects in scene.
        Transform[] selectedObjects = Selection.transforms;

        Vector3 averagePosition = Vector3.zero;
        bool nestParent = true;
        Transform newGroupTransform = new GameObject("Group").transform;

        Undo.RegisterCreatedObjectUndo(newGroupTransform, "Create group parent");

        Transform coreParent = selectedObjects[0].parent;
        foreach(Transform item in selectedObjects) {
            if(item.parent != coreParent) {
                nestParent = false;
            }
            averagePosition += item.position;
        }

        if(nestParent) {
            Undo.SetTransformParent(newGroupTransform, coreParent, "Set group parent's parent");
            newGroupTransform.position = averagePosition / selectedObjects.Length;
        }
        else {
            //Place group's pivot on the active transform in the scene.
            newGroupTransform.position = Selection.activeTransform.position;
        }

        //Set selected objects as children of the group.
        foreach(Transform item in selectedObjects) {
            Undo.SetTransformParent(item, newGroupTransform, "Group Selected");
        }
    }

    [MenuItem("Utility/Ungroup Selected %#g", false, 2)]
    static void UngroupSelected()
    {
        Transform activeSelection = Selection.activeTransform;

        Transform selectedParent = activeSelection.parent;

        //Store each child in a list, since otherwise we'd mess with the
        //enumerable state while enumerating it.
        List<Transform> toUnparent = new List<Transform>();

        foreach(Transform child in activeSelection) {
            toUnparent.Add(child);
        }

        foreach(Transform child in toUnparent) {
            Undo.SetTransformParent(child, selectedParent, "Ungroup " + activeSelection.name);
        }

        if(activeSelection.GetComponents<Component>().Length > 1) {
            Debug.Log("Ungroup " + activeSelection.name + ": not deleting the group parent because it has other components on it!\nDelete it manually if you need to.");
        }
        else {
            Undo.DestroyObjectImmediate(activeSelection.gameObject);
        }
    }
}
