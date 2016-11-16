using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;
using System.Linq;
using System.Reflection;
using System.IO;

public class ScriptableObjectWizard : EditorWindow
{
    static ScriptableObjectWizard window;

    static Type[] scriptableObjectTypes;
    static string[] scriptableObjectTypeNames;

    [MenuItem("Utility/Scriptable Object Wizard", false, 350)]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = EditorWindow.GetWindow<ScriptableObjectWizard>(true, "Scriptable Object Wizard");

        scriptableObjectTypes = Assembly
            .GetAssembly(typeof(Util))
            .GetTypes()
            .Where(t => typeof(ScriptableObject).IsAssignableFrom(t) && t != typeof(ScriptableObject))
            .ToArray();

        scriptableObjectTypeNames = scriptableObjectTypes.Select(t => t.Name).ToArray();
    }

    int selectedIndex = -1;
    string filename;

    void OnGUI()
    {
        int newSelectedIndex = EditorGUILayout.Popup("Scriptable Object Type", selectedIndex, scriptableObjectTypeNames);
        if(newSelectedIndex != selectedIndex) {
            if(string.IsNullOrEmpty(filename) || filename == "New" + scriptableObjectTypes[selectedIndex].Name) {
                filename = "New" + scriptableObjectTypes[newSelectedIndex].Name;
            }
            selectedIndex = newSelectedIndex;
        }
        filename = EditorGUILayout.TextField("Filename", filename);

        string error = GetError();

        if(error != null) {
            EditorGUILayout.HelpBox(error, MessageType.Warning);
        }
        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(error != null);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Create")) {
            var so = ScriptableObject.CreateInstance(scriptableObjectTypes[selectedIndex]);
            AssetDatabase.CreateAsset(so, "Assets/" + filename + ".asset");
            window.Close();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
        
    }

    string GetError()
    {
        if(selectedIndex < 0)
            return "Choose a Scriptable Object type!";
        if(string.IsNullOrEmpty(filename))
            return "Filename cannot be empty!";
        if(File.Exists("Assets/" + filename + ".asset"))
            return "File already exists!";

        return null;
    }
}
