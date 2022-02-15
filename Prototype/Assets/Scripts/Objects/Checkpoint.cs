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
