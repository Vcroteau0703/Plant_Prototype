using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Volume : MonoBehaviour, ISavable
{
    public bool oneShot;
    private bool activated;
    public LayerMask Layer_Filter;

    public delegate void Action(GameObject actor);
    public Action action;

    private void Awake()
    {
        Load();      
    }

    private void OnEnable()
    {
        if (oneShot)
        {
            action += Set_Activated;
        }
    }

    public enum Trigger_Type { 
        Trigger_Stay, 
        Trigger_Enter, 
        Trigger_Exit, 
        Collision_Stay, 
        Collision_Enter, 
        Collision_Exit
    }

    public Trigger_Type trigger_type;

    public void OnTriggerStay(Collider other)
    {       
        if ((Layer_Filter & (1<<other.gameObject.layer)) == 0){ return; }
        else if (trigger_type == Trigger_Type.Trigger_Stay) { action.Invoke(other.gameObject); }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hello");
        if ((Layer_Filter & (1 << other.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Trigger_Enter) { action.Invoke(other.gameObject); }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if ((Layer_Filter & (1 << other.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Trigger_Exit) { action.Invoke(other.gameObject); }
    }

    public void OnCollisionStay(Collision collision)
    {
        if ((Layer_Filter & (1 << collision.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Collision_Stay) { action.Invoke(collision.gameObject); }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((Layer_Filter & (1 << collision.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Collision_Enter) { action.Invoke(collision.gameObject); }
    }

    public void OnCollisionExit(Collision collision)
    {
        if ((Layer_Filter & (1 << collision.gameObject.layer)) == 0) { return; }
        else if (trigger_type == Trigger_Type.Collision_Exit) { action.Invoke(collision.gameObject); }
    }

    public void Set_Activated(GameObject actor)
    {
        action = Set_Activated;
        activated = true;
    }

    public void Save()
    {
        Action_Volume_Data data = new Action_Volume_Data(activated);
        SaveSystem.Save(data, "/Levels/actionVol_" + transform.position.magnitude.ToString() + ".data");
    }

    public void Load()
    {
        Action_Volume_Data data = SaveSystem.Load<Action_Volume_Data>("/Levels/actionVol_" + transform.position.magnitude.ToString() + ".data");
        if (data != null && data.activated) { Destroy(gameObject); }
    }
}

[System.Serializable]
public class Action_Volume_Data 
{
    public bool activated;
    public Action_Volume_Data(bool activated) { this.activated = activated; }
}

