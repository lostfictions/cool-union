using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class MenuOpenSublimeProject
{
    [MenuItem("File/Open Sublime Project", false, 0)]
    static void OpenSublimeProject()
    {
        var files = Directory.GetFiles(".", "*.sublime-project");

        if(files.Length > 0) {
            Process.Start(files[0]);
        }
        else {
            Debug.LogError("No sublime-project file found!");
        }
    }
}
