using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RockWallSave : MonoBehaviour
{
    public PlayableDirector cutscene;
    public bool isDone = false;

    public void Save()
    {
        RockWallData data = new RockWallData();

        if (isDone)
        {
            data.isDone = true;
        }

        SaveSystem.Save<RockWallData>(data, "/Level01_RockWall.data");

        //throw new System.NotImplementedException();
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

