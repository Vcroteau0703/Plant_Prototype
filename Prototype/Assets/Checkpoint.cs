using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Checkpoint : MonoBehaviour, ICheckpoint
{
    public Bounds bounds;
    public LayerMask trigger_filter;
    public enum State { Disabled = default, Active, Inactive}
    public State state = default;
    private void Update()
    {
        if (state == State.Inactive)
        {
            Collider[] hit = Physics.OverlapBox(transform.position + bounds.center, bounds.extents, Quaternion.identity, trigger_filter);
            if(hit.Length > 0)
            {
                foreach(MonoBehaviour m in FindObjectsOfType<MonoBehaviour>())
                {
                    if(m.TryGetComponent(out ICheckpoint c)){
                        c.Update_Checkpoint(this);
                    }
                }
            }
        }
    }

    public Checkpoint Get_Active_Checkpoint()
    {
        if(state == State.Active) { return this; }
        return null;
    }


    public void Update_Checkpoint(Checkpoint checkpoint)
    {
        if(checkpoint == this)
        {
            state = State.Active;
        }
        else
        {
            state = State.Inactive;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.4f);
        Gizmos.DrawCube(transform.position, Vector3.one);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    #region EDITOR

    [MenuItem("GameObject/Custom/Checkpoint", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
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
    #endregion
}
