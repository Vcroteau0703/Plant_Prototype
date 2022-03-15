using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RockWallSave : MonoBehaviour, ISavable
{
    public PlayableDirector cutscene;
    public bool isDone = false;

    public void Save()
    {
        RockWallData data = SaveSystem.Load<RockWallData>("/Level01_RockWall.data");

        if (data == null && isDone)
        {
            data = new RockWallData();
            data.isDone = true;
            SaveSystem.Save<RockWallData>(data, "/Level01_RockWall.data");
        }

    }

    public void Load()
    {
        RockWallData data = SaveSystem.Load<RockWallData>("/Level01_RockWall.data");

        if (data != null && data.isDone)
        {
            cutscene.initialTime = 240f;
            cutscene.Play();
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
public class RockWallData
{
    public bool isDone = false;
}

