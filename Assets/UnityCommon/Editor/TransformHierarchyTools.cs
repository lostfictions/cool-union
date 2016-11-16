using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;

using System.Linq;
using System.Text;

using System.Reflection;

public static class TransformHierarchyTools
{
    static Transform copiedHierarchy;

    [MenuItem("Utility/Copy Hierarchy", false, 120)]
    static void CopyHierarchy()
    {
        var selection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        copiedHierarchy = selection[0];
        Debug.Log("Copied " + copiedHierarchy.name + ".");
    }

    [MenuItem("Utility/Paste Hierarchy and Components Additively", false, 121)]
    static void PasteHierarchyAdditively()
    {
        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        var report = new StringBuilder();
        report.AppendLine("Paste Hierarchy Additively Report:");

        var componentsToVerify = new List<Component>();

        RecursiveAdditivePaste(copiedHierarchy, newSelection[0], componentsToVerify, report);

        VerifyAndRewireComponents(copiedHierarchy, newSelection[0], componentsToVerify, report);

        Debug.Log(report.ToString());
    }

    [MenuItem("Utility/Paste Components Additively", false, 122)]
    static void PasteComponentsAdditively()
    {
        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        var report = new StringBuilder();
        report.AppendLine("Paste Components Additively Report:");

        var componentsToVerify = new List<Component>();

        TransferComponents(copiedHierarchy, newSelection[0], componentsToVerify, report);

        VerifyAndRewireComponents(copiedHierarchy, newSelection[0], componentsToVerify, report);

        Debug.Log(report.ToString());
    }

    public static void RecursiveAdditivePaste(Transform source, Transform destination, List<Component> componentsToVerify, StringBuilder report)
    {
        TransferComponents(source, destination, componentsToVerify, report);

        var sourceChildren = source.Cast<Transform>().OrderBy(c => c.name);
        var destinationChildren = destination.Cast<Transform>().OrderBy(c => c.name);

        foreach(var sourceChild in sourceChildren) {
            var destMatch = destinationChildren.FirstOrDefault(c => c.name == sourceChild.name);
            if(destMatch != null) {
                RecursiveAdditivePaste(sourceChild, destMatch, componentsToVerify, report);
            }
            else {
                var newChild = (Transform)Object.Instantiate(sourceChild, sourceChild.position, sourceChild.rotation);
                newChild.name = sourceChild.name;
                newChild.parent = destination.transform;
                newChild.localPosition = sourceChild.localPosition;
                newChild.localRotation = sourceChild.localRotation;
                newChild.localScale = sourceChild.localScale;

                Undo.RegisterCreatedObjectUndo(newChild.gameObject, "Paste Hierarchy Additively");
                report.AppendLine("Added " + newChild.name + " to " + destination.name + ";");

                var newComponentsToVerify = newChild.GetComponentsInChildren<Component>(true).Where(c => !(c is Transform)).ToArray();
                if(newComponentsToVerify.Any()) {
                    componentsToVerify.AddRange(newComponentsToVerify);
                    report.AppendLine();
                    report.Append("Queued components to verify: ");
                    foreach(var c in newComponentsToVerify) {
                        report.Append(c.GetType().Name + " on " + c.name + "; ");
                    }
                }
            }
        }
    }

    // Limitations:
    // - Only transfers one of each component type (ie. won't paste two instances of the same script onto the destination.)
    // - Will only transfer over public fields (ie. will miss non-public but serializable fields on scripts.)
    static void TransferComponents(Transform source, Transform destination, List<Component> componentsToVerify, StringBuilder report)
    {
        report.AppendLine("\nTransfering components from " + source.name + " to " + destination.name + ".");

        var sourceComponents = source.GetComponents<Component>().Where(c => !(c is Transform));
        var destComponents = destination.GetComponents<Component>().Where(c => !(c is Transform)).ToArray();

        foreach(var sc in sourceComponents) {
            var componentType = sc.GetType();
            var destMatch = destComponents.FirstOrDefault(dc => dc.GetType() == componentType);
            if(destMatch == null) {
                if(!UnityEditorInternal.ComponentUtility.CopyComponent(sc)) {
                    Debug.Log("\tError copying component " + componentType.Name + " from " + sc.name);
                    continue;
                }
                if(!UnityEditorInternal.ComponentUtility.PasteComponentAsNew(destination.gameObject)) {
                    Debug.Log("\tError pasting component " + componentType.Name + " to " + destination.name);
                    continue;
                }

                var newComponent = destination.GetComponent(componentType);

                //Pretty sure PasteComponentAsNew handles this itself.
                //Undo.RegisterCreatedObjectUndo(newComponent, "Paste Component Additively");

                report.AppendLine("\tCreated " + componentType.Name + " on " + destination.name + ";");

                componentsToVerify.Add(newComponent);
            }
        }

        report.AppendLine("\tChanging tag from '" + destination.tag + "' to '" + source.tag + "' and " +
                          "layer from '" + destination.gameObject.layer + "' to '" + source.gameObject.layer + "'.");
        destination.tag = source.tag;
        destination.gameObject.layer = source.gameObject.layer;
    }

    //We perform this as a separate step at the end of any paste because we only want to
    //verify components after all possible references have also been pasted to the destination.
    public static void VerifyAndRewireComponents(Transform sourceRoot, Transform destinationRoot, IEnumerable<Component> components, StringBuilder report)
    {
        foreach(var c in components) {
            report.AppendLine("\nVerifying and rewiring fields on " + destinationRoot.name + "'s " + c.GetType().Name);

            bool componentDidChange;

            if(c is MonoBehaviour) {
                componentDidChange = RewireMonoBehaviour(sourceRoot, destinationRoot, report, c);
            }
            else {
                componentDidChange = RewireBuiltInComponent(sourceRoot, destinationRoot, report, c);
            }

            if(componentDidChange) {
                EditorUtility.SetDirty(c);
            }
        }
    }

    static bool RewireBuiltInComponent(Transform sourceRoot, Transform destinationRoot, StringBuilder report, Component component)
    {
        bool componentDidChange = false;

        var props = component.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => pi.CanWrite && pi.CanRead)
            .Where(p => typeof(Component).IsAssignableFrom(p.PropertyType));
        

        foreach(var prop in props) {
            var propertyType = prop.PropertyType;
            if(typeof(Component).IsAssignableFrom(propertyType)) {
                var fieldValue = (Component)prop.GetGetMethod().Invoke(component, new object[] {});

                report.AppendLine("Verifying the '" + prop.Name + "' (" + propertyType.Name + ") prop of component '" + component.GetType().Name + "' on " + component.name + ", parented to " + component.transform.root.name);
                if(fieldValue == null) {
                    report.AppendLine("\tIgnoring '" + prop.Name + "', since its value is null.");
                    continue;
                }

                if(fieldValue.transform.root != sourceRoot) {
                    report.AppendLine("\tIgnoring '" + prop.Name + "', since its root is " + fieldValue.transform.root + " instead of " + sourceRoot);
                    continue;
                }

                string fieldPath = GetTransformPath(fieldValue);

                //We need to cut the first segment from the path, since it's the root
                string truncatedFieldPath = fieldPath.Substring(fieldPath.IndexOf('/') + 1);

                Transform newTargetTransform;
                if(truncatedFieldPath == sourceRoot.name) {
                    //The component is on the root of the object, so let's use that.
                    newTargetTransform = destinationRoot;
                }
                else {
                    newTargetTransform = destinationRoot.Find(truncatedFieldPath);
                }

                if(newTargetTransform == null) {
                    report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + prop.Name + "' on " +
                                      component.name + " from " + fieldPath + ", but there was no corresponding transform at the destination.");
                    continue;
                }
                var newTargetComponent = newTargetTransform.GetComponent(propertyType);
                if(newTargetComponent == null) {
                    report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + prop.Name + "' on " +
                                      component.name + " from " + fieldPath + ", but there was no corresponding component at the destination.");
                    continue;
                }

                //This one isn't necessary since we're already recording the component creation!
                //Undo.RecordObject(component, "Rewire component values");

                prop.GetSetMethod().Invoke(component, new object[] {newTargetComponent});
                componentDidChange = true;

                string newPath = fieldPath.Replace(sourceRoot.name, destinationRoot.name);
                report.AppendLine("\tRewired component " + component.GetType().Name + "'s field '" + prop.Name + "' on " + component.name + " from " + fieldPath + " to " + newPath);
            }
        }
        return componentDidChange;
    }

    static bool RewireMonoBehaviour(Transform sourceRoot, Transform destinationRoot, StringBuilder report, Component component)
    {
        bool componentDidChange = false;
        var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach(var field in fields) {
            var fieldType = field.FieldType;
            if(typeof(Component).IsAssignableFrom(fieldType)) {
                var fieldValue = (Component)field.GetValue(component);

                report.AppendLine("Verifying the '" + field.Name + "' (" + fieldType.Name + ") prop of component '" + component.GetType().Name + "' on " + component.name + ", parented to " + component.transform.root.name);
                if(fieldValue == null) {
                    report.AppendLine("\tIgnoring '" + field.Name + "', since its value is null.");
                    continue;
                }

                if(fieldValue.transform.root != sourceRoot) {
                    report.AppendLine("\tIgnoring '" + field.Name + "', since its root is " + fieldValue.transform.root + " instead of " + sourceRoot);
                    continue;
                }

                string fieldPath = GetTransformPath(fieldValue);

                //We need to cut the first segment from the path, since it's the root
                string truncatedFieldPath = fieldPath.Substring(fieldPath.IndexOf('/') + 1);

                Transform newTargetTransform;
                if(truncatedFieldPath == sourceRoot.name) {
                    //The component is on the root of the object, so let's use that.
                    newTargetTransform = destinationRoot;
                }
                else {
                    newTargetTransform = destinationRoot.Find(truncatedFieldPath);
                }

                if(newTargetTransform == null) {
                    report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + field.Name + "' on " +
                                      component.name + " from " + fieldPath + ", but there was no corresponding transform at the destination.");
                    continue;
                }
                var newTargetComponent = newTargetTransform.GetComponent(fieldType);
                if(newTargetComponent == null) {
                    report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + field.Name + "' on " +
                                      component.name + " from " + fieldPath + ", but there was no corresponding component at the destination.");
                    continue;
                }

                Undo.RecordObject(component, "Rewire component values");
                field.SetValue(component, newTargetComponent);
                componentDidChange = true;

                string newPath = fieldPath.Replace(sourceRoot.name, destinationRoot.name);
                report.AppendLine("\tRewired component " + component.GetType().Name + "'s field '" + field.Name + "' on " + component.name + " from " + fieldPath + " to " + newPath);
            }
        }
        return componentDidChange;
    }

    public static string GetTransformPath(Component c)
    {
        var sb = new StringBuilder();

        var t = c.transform;
        sb.Append(t.name);

        while(t.parent != null) {
            t = t.parent;
            sb.Insert(0, t.name + "/");
        }
        return sb.ToString();
    }


    #region Menu verification methods
    [MenuItem("Utility/Copy Hierarchy", true, 120)]
    static bool CanCopyHierarchy()
    {
        var selection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        return selection.Length == 1;
    }

    [MenuItem("Utility/Paste Hierarchy and Components Additively", true, 121)]
    static bool CanPasteHierarchyAdditively()
    {
        return CanPaste();
    }

    [MenuItem("Utility/Paste Components Additively", true, 122)]
    static bool CanPasteComponentsAdditively()
    {
        return CanPaste();
    }

    static bool CanPaste()
    {
        if(copiedHierarchy == null) {
            return false;
        }

        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        if(newSelection.Length != 1) {
            return false;
        }

        return true;
    }
    #endregion

}
