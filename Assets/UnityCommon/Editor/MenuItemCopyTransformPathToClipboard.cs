using System.Text;
using UnityEngine;
using System.Collections.Generic;

using UnityEditor;

public static class MenuItemCopyTransformPathToClipboard
{
    [MenuItem("Utility/Copy Transform Path to Clipboard", true, 150)]
    static bool CanCopyTransformPathToClipboard()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem("Utility/Copy Transform Path to Clipboard", false, 150)]
    static void CopyTransformPathToClipboard()
    {
        var selection = Selection.activeTransform;

        var sb = new StringBuilder();
        sb.Append(selection.name);
        while(selection.parent != null) {
            selection = selection.parent;
            sb.Insert(0, selection.name + "/");
        }
        EditorGUIUtility.systemCopyBuffer = sb.ToString();
    }
}
