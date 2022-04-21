using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSave : MonoBehaviour
{
    public string SaveFile;
    public bool isDone = false;
    public GameObject trigger;
    public GameObject character;

    public void Save()
    {
        TriggerData data = SaveSystem.Load<TriggerData>(SaveFile);

        if (data == null && isDone)
        {
            data = new TriggerData();
            data.isDone = true;
            SaveSystem.Save<TriggerData>(data, SaveFile);
        }

    }

    public void Load()
    {
        TriggerData data = SaveSystem.Load<TriggerData>(SaveFile);

        if (data != null && data.isDone)
        {
            if(character != null)
            {
                character.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        Load();
    }

    public void CutsceneDone(bool done)
    {
        isDone = done;
    }


}
[System.Serializable]
public class TriggerData
{
    public bool isDone = false;
}
