using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Custom_Editor : Editor
{
    [MenuItem("GameObject/Custom/Checkpoint", false, 10)]
    static void Create_Checkpoint(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("Checkpoint");
        go.AddComponent<Checkpoint>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Custom/Wind Volume", false, 10)]
    static void Create_Wind_Volume(MenuCommand menuCommand)
    {
        GameObject prefab = Resources.Load<Custom_Editor_Data>("Data/Custom Editor Data").wind_volume;
        // Create a custom game object
        GameObject go = Instantiate(prefab);
        go.name = "Wind Volume";
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Custom/Portal Volume", false, 10)]
    static void Create_Portal_Volume(MenuCommand menuCommand)
    {
        GameObject prefab = Resources.Load<Custom_Editor_Data>("Data/Custom Editor Data").portal_volume;
        // Create a custom game object
        GameObject go = Instantiate(prefab);
        go.name = "Portal Volume";
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
