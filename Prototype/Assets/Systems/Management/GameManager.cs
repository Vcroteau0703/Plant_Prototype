using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISavable
{
    public static GameManager instance;

    public SceneData sceneData;
    public static void SaveGame(bool show_notify)
    {
        MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>(true);
        foreach (MonoBehaviour m in monos)
        {
            foreach(ISavable a in m.GetComponents<ISavable>())
            {
                a.Save();
            }
        }

        if (show_notify)
        {
            Notification_System.Send_SystemNotify("The game has been saved");
        }
    }
    public static void SaveGame()
    {
        MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>(true);
        foreach (MonoBehaviour m in monos)
        {
            foreach (ISavable a in m.GetComponents<ISavable>())
            {
                a.Save();
            }
        }
        Notification_System.Send_SystemNotify("The game has been saved");       
    }

    private void Awake()
    {      
        instance = this;
        sceneData = new SceneData();
        Load();
    }

    public void Save()
    {
        SaveSystem.Save(sceneData, "/Levels/" + SceneManager.GetActiveScene().name + ".data");
    }

    public void Load()
    {
        SceneData data = SaveSystem.Load<SceneData>("/Levels/" + SceneManager.GetActiveScene().name + ".data");
        sceneData = data != null ? data : new SceneData();
    }
}

[System.Serializable]
public class SceneData
{
    public Action_Volume_Data[] action_volumes = new Action_Volume_Data[0];
    public void Update_AVol_Data(Action_Volume_Data data)
    {
        for(int i = 0; i < action_volumes.Length; i++)
        {
            if(data.ID == action_volumes[i].ID)
            {
                action_volumes[i] = data;
                return;
            }
        }
        Add_AVol_Data(data);
    }
    public void Add_AVol_Data(Action_Volume_Data data)
    {
        Action_Volume_Data[] temp = new Action_Volume_Data[action_volumes.Length + 1];

        int i = 0;
        for(i = 0; i < action_volumes.Length; i++)
        {
            temp[i] = action_volumes[i];
        }
        temp[temp.Length-1] = data;
        action_volumes = temp;
    }
    public Action_Volume_Data Get_AVol_Data(int id)
    {
        foreach(Action_Volume_Data a in action_volumes)
        {
            if(a.ID == id)
            {
                return a;
            }
        }
        return null;
    }
}
