using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BoulderSave : MonoBehaviour, ISavable
{
    public PlayableDirector cutscene;
    public bool isDone = false;

    public void Save()
    {
        BoulderData data = new BoulderData();

        if (isDone)
        {
            data.isDone = true;
        }

        SaveSystem.Save<BoulderData>(data, "/Level01_boulder.data");

        throw new System.NotImplementedException();
    }

    public void Load()
    {
        BoulderData data = SaveSystem.Load<BoulderData>("/Level01_boulder.data");

        if(data != null && data.isDone)
        {
            cutscene.initialTime = 250f;
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
public class BoulderData
{
    public bool isDone = false;
}
