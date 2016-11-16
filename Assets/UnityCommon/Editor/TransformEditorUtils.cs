// A combination of http://wiki.unity3d.com/index.php/Blender_Camera_Controls
// and a transform reset helper adapted from NGUI. We need to combine them
// because we can't have more than one custom editor script for a single type.

using UnityEngine;
using UnityEditor;

// Numpad controls:
//
// Numpad1 				= Front view
// Control + Numpad1	= Rear view 
// Numpad3				= Right view
// Control + Numpad3	= Left view 
// Numpad7				= Top view
// Control + Numpad7	= Bottom view
// Numpad8				= Rotate view up
// Numpad2				= Rotate view down
// Numpad4				= Rotate view left
// Numpad6				= Rotate view right
// Numpad5				= Toggle orthographic / perspective
// Numpad.				= Center view on object(s)
// Numpad+				= Zoom camera in
// Numpad-				= Zoom camera out
// Numpad0				= Focus selected objects (same as Unity's F key)

[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
public class TransformEditorUtils : Editor
{
    /// Numpad controls
    UnityEditor.SceneView sceneView;

    Vector3 eulerAngles;
    Event current;
    Quaternion rotHelper;

    public void OnSceneGUI()
    {
        current = Event.current;

        if(!current.isKey || current.type != EventType.keyDown) {
            return;
        }

        sceneView = UnityEditor.SceneView.lastActiveSceneView;
        eulerAngles = sceneView.camera.transform.rotation.eulerAngles;
        rotHelper = sceneView.camera.transform.rotation;

        switch(current.keyCode) {
            case KeyCode.Keypad1:
                if(current.control == false) {
                    sceneView.LookAtDirect(
                        SceneView.lastActiveSceneView.pivot,
                        Quaternion.Euler(new Vector3(0f, 360f, 0f)));
                }
                else {
                    sceneView.LookAtDirect(
                        SceneView.lastActiveSceneView.pivot,
                        Quaternion.Euler(new Vector3(0f, 180f, 0f)));
                }
                break;
            case KeyCode.Keypad2:
                sceneView.LookAtDirect(
                    SceneView.lastActiveSceneView.pivot,
                    rotHelper * Quaternion.Euler(new Vector3(-15f, 0f, 0f)));
                break;
            case KeyCode.Keypad3:
                if(current.control == false) {
                    sceneView.LookAtDirect(
                        SceneView.lastActiveSceneView.pivot,
                        Quaternion.Euler(new Vector3(0f, 270f, 0f)));
                }
                else {
                    sceneView.LookAtDirect(
                        SceneView.lastActiveSceneView.pivot,
                        Quaternion.Euler(new Vector3(0f, 90f, 0f)));
                }
                break;
            case KeyCode.Keypad4:
                sceneView.LookAtDirect(
                    SceneView.lastActiveSceneView.pivot,
                    Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + 15f, eulerAngles.z)));
                break;
            case KeyCode.Keypad5:
                sceneView.orthographic = !sceneView.orthographic;
                break;
            case KeyCode.Keypad6:
                sceneView.LookAtDirect(
                    SceneView.lastActiveSceneView.pivot,
                    Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y - 15f, eulerAngles.z)));
                break;
            case KeyCode.Keypad7:
                if(current.control == false) {
                    sceneView.LookAtDirect(
                        SceneView.lastActiveSceneView.pivot,
                        Quaternion.Euler(new Vector3(90f, 0f, 0f)));
                }
                else {
                    sceneView.LookAtDirect(
                        SceneView.lastActiveSceneView.pivot,
                        Quaternion.Euler(new Vector3(270f, 0f, 0f)));
                }
                break;
            case KeyCode.Keypad8:
                sceneView.LookAtDirect(
                    SceneView.lastActiveSceneView.pivot,
                    rotHelper * Quaternion.Euler(new Vector3(15f, 0f, 0f)));
                break;
            case KeyCode.KeypadPeriod:
                if(Selection.transforms.Length == 1) {
                    sceneView.LookAtDirect(Selection.activeTransform.position, sceneView.camera.transform.rotation);
                }
                else if(Selection.transforms.Length > 1) {
                    Vector3 tempVec = new Vector3();
                    for(int i = 0; i < Selection.transforms.Length; i++) {
                        tempVec += Selection.transforms[i].position;
                    }
                    sceneView.LookAtDirect((tempVec / Selection.transforms.Length), sceneView.camera.transform.rotation);
                }
                break;
            case KeyCode.KeypadMinus:
                SceneView.RepaintAll();
                sceneView.size *= 1.1f;
                break;
            case KeyCode.KeypadPlus:
                SceneView.RepaintAll();
                sceneView.size /= 1.1f;
                break;
            case KeyCode.Keypad0:
                sceneView.FrameSelected();
                break;
        }
    }

    /// End numpad controls, begin transform reset
    SerializedProperty mPos;

    SerializedProperty mRot;
    SerializedProperty mScale;

    void OnEnable()
    {
        mPos = serializedObject.FindProperty("m_LocalPosition");
        mRot = serializedObject.FindProperty("m_LocalRotation");
        mScale = serializedObject.FindProperty("m_LocalScale");
    }

    /// <summary>
    /// Draw the inspector widget.
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 15f;

        serializedObject.Update();

        DrawPosition();
        DrawRotation();
        DrawScale();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawPosition()
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button("P", GUILayout.Width(20f));

            EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));

            if(reset) {
                mPos.vector3Value = Vector3.zero;
            }
        }
        GUILayout.EndHorizontal();
    }

    void DrawScale()
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button("S", GUILayout.Width(20f));

            EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
            EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
            EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));

            if(reset) {
                mScale.vector3Value = Vector3.one;
            }
        }
        GUILayout.EndHorizontal();
    }

    #region Rotation is ugly as hell... since there is no native support for quaternion property drawing

    enum Axes : int
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = 7,
    }

    Axes CheckDifference(Transform t, Vector3 original)
    {
        Vector3 next = t.localEulerAngles;

        Axes axes = Axes.None;

        if(Differs(next.x, original.x)) {
            axes |= Axes.X;
        }
        if(Differs(next.y, original.y)) {
            axes |= Axes.Y;
        }
        if(Differs(next.z, original.z)) {
            axes |= Axes.Z;
        }

        return axes;
    }

    Axes CheckDifference(SerializedProperty property)
    {
        Axes axes = Axes.None;

        if(property.hasMultipleDifferentValues) {
            Vector3 original = property.quaternionValue.eulerAngles;

            foreach(Object obj in serializedObject.targetObjects) {
                axes |= CheckDifference(obj as Transform, original);
                if(axes == Axes.All) {
                    break;
                }
            }
        }
        return axes;
    }

    /// <summary>
    /// Draw an editable float field.
    /// </summary>
    /// <param name="hidden">Whether to replace the value with a dash</param>
    static bool FloatField(string name, ref float value, bool hidden, GUILayoutOption opt)
    {
        float newValue = value;
        GUI.changed = false;

        if(!hidden) {
            newValue = EditorGUILayout.FloatField(name, newValue, opt);
        }
        else {
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
        }

        if(GUI.changed && Differs(newValue, value)) {
            value = newValue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Because Mathf.Approximately is too sensitive.
    /// </summary>
    static bool Differs(float a, float b)
    {
        return Mathf.Abs(a - b) > 0.0001f;
    }

    void DrawRotation()
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button("R", GUILayout.Width(20f));

            Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;
            Axes changed = CheckDifference(mRot);
            Axes altered = Axes.None;

            GUILayoutOption opt = GUILayout.MinWidth(30f);

            if(FloatField("X", ref visible.x, (changed & Axes.X) != 0, opt)) {
                altered |= Axes.X;
            }
            if(FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, opt)) {
                altered |= Axes.Y;
            }
            if(FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, opt)) {
                altered |= Axes.Z;
            }

            if(reset) {
                mRot.quaternionValue = Quaternion.identity;
            }
            else if(altered != Axes.None) {
                Undo.RecordObjects(serializedObject.targetObjects, "Change Rotation");

                foreach(Object obj in serializedObject.targetObjects) {
                    Transform t = obj as Transform;
                    Vector3 v = t.localEulerAngles;

                    if((altered & Axes.X) != 0) {
                        v.x = visible.x;
                    }
                    if((altered & Axes.Y) != 0) {
                        v.y = visible.y;
                    }
                    if((altered & Axes.Z) != 0) {
                        v.z = visible.z;
                    }

                    t.localEulerAngles = v;
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    #endregion
}
