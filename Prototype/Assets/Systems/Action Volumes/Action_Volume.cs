using UnityEngine;
using UnityEngine.SceneManagement;

public class Action_Volume : MonoBehaviour
{
    public Action_Volume_Data data;

    public bool oneShot;
    public LayerMask Layer_Filter;

    public delegate void Action(GameObject actor);
    public Action action;

    private void Awake()
    {
        Load();      
    }

    protected void OnEnable()
    {
        if (oneShot == true)
        {
            action = Save_And_Destroy;
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

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name);
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

    public void Save_And_Destroy(GameObject actor)
    {       
        data.activated = true;
        data.ID = ((int)transform.position.sqrMagnitude);
        GameManager.instance.sceneData.Update_AVol_Data(data);
        Destroy(gameObject);
    }

    public void Load()
    {
        SceneData sceneData = SaveSystem.Load<SceneData>("/Levels/" + SceneManager.GetActiveScene().name + ".data");
        Action_Volume_Data data = sceneData != null ? sceneData.Get_AVol_Data((int)transform.position.sqrMagnitude) : null;
        if (data != null && data.activated) { Destroy(gameObject); }
    }
}

[System.Serializable]
public class Action_Volume_Data 
{
    public int ID;
    public bool activated;    
    public Action_Volume_Data(int ID, bool activated) {
        this.ID = ID;
        this.activated = activated;
    }
}

