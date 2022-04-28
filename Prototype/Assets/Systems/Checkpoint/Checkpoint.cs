using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, ICheckpoint
{
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 2);
    public LayerMask trigger_filter;
    public enum State { Inactive = default, Disabled, Active}
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
    public static Checkpoint Get_Active_Checkpoint()
    {
        foreach (MonoBehaviour m in FindObjectsOfType<MonoBehaviour>())
        {
            Checkpoint x = null;
            if (m.TryGetComponent(out Checkpoint c))
            {
                if(c.state == State.Active) { return c; }
            }
        }
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
        if (state == State.Active)
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.4f);
            Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
        }
        else
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.4f);
            Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
        }
    }
}

[System.Serializable]
public class Checkpoint_Data
{
    public float[] position;

    public Checkpoint_Data(Checkpoint c)
    {
        position = new float[3];
        position[0] = c.transform.position.x;
        position[1] = c.transform.position.y;
        position[2] = c.transform.position.z;
    }
}
