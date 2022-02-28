using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Volume : MonoBehaviour
{
    public LayerMask Layer_Filter;

    public delegate void Action(GameObject actor);
    public Action action;

    public enum Trigger_Type { 
        Trigger_Stay, 
        Trigger_Enter, 
        Trigger_Exit, 
        Collision_Stay, 
        Collision_Enter, 
        Collision_Exit
    }

    public Trigger_Type trigger_type;

    private void OnTriggerStay(Collider other)
    {       
        if ((Layer_Filter & (1<<other.gameObject.layer)) == 0){ return; }
        else if (trigger_type == Trigger_Type.Trigger_Stay) { action.Invoke(other.gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((Layer_Filter & (1 << other.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Trigger_Enter) { action.Invoke(other.gameObject); }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((Layer_Filter & (1 << other.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Trigger_Exit) { action.Invoke(other.gameObject); }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((Layer_Filter & (1 << collision.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Collision_Stay) { action.Invoke(collision.gameObject); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((Layer_Filter & (1 << collision.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Collision_Enter) { action.Invoke(collision.gameObject); }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((Layer_Filter & (1 << collision.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Collision_Exit) { action.Invoke(collision.gameObject); }
    }
}
