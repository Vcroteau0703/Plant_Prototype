using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Boulder : Hazard, ISavable
{
    public PlayableDirector director;
    public string boulderSaveID;
    public bool isDone = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable a))
        {
            a.Damage(damage);
            if(damage > 0 && director != null)
            {
                ResetCutscene();
            }
        }
    }

    public void ResetCutscene()
    {
        director.time = 0;
        director.Pause();
    }

    public void Save()
    {
        BoulderData data = SaveSystem.Load<BoulderData>(boulderSaveID);

        if (data == null && isDone)
        {
            data = new BoulderData();
            data.isDone = true;
            SaveSystem.Save<BoulderData>(data, boulderSaveID);
        }
    }

    public void Load()
    {
        BoulderData data = SaveSystem.Load<BoulderData>(boulderSaveID);

        if (data != null && data.isDone)
        {
            director.initialTime = 250f;
            director.Play();
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