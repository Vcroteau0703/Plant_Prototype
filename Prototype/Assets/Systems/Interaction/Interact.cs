using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Interact : MonoBehaviour
{
    public Interact_Data data;

    public bool repeatable;

    public UnityEvent OnInteractTriggered;

    protected void Awake()
    {
        Load();
        if (!repeatable)
        {
            OnInteractTriggered.AddListener(Save_And_Destroy);
        }
    }

    public void Save_And_Destroy()
    {
        data.active = true;
        data.ID = ((int)transform.position.sqrMagnitude);
        GameManager.instance.sceneData.Update_Scene_Data(data);
        //Destroy(gameObject);
    }

    public void Load()
    {
        SceneData sceneData = SaveSystem.Load<SceneData>("/Levels/" + SceneManager.GetActiveScene().name + ".data");
        Interact_Data data = sceneData != null ? sceneData.Get_Interact_Data((int)transform.position.sqrMagnitude) : null;
        if (data != null && data.active) { Destroy(gameObject); }
    }

    public void TriggerEvent()
    {
        OnInteractTriggered.Invoke();
    }
}

[System.Serializable]
public class Interact_Data
{
    public int ID;
    public bool active;
    public Interact_Data(int ID, bool active)
    {
        this.ID = ID;
        this.active = active;
    }
}


