using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene_Save : MonoBehaviour
{
    public PlayableDirector cutscene;

    public void Load()
    {
        Cutscene_Save_Data data = SaveSystem.Load<Cutscene_Save_Data>("/Levels/Cutscene_Save.data");

        if (data == null || data.cutscenes == null) { return; }

        foreach (Cutscene_Data saved in data.cutscenes)
        {
            if(gameObject.name == saved.id)
            {
                cutscene.initialTime = cutscene.duration;
                cutscene.Play();
            }
        }
    }

    private void Awake()
    {
        cutscene = GetComponent<PlayableDirector>();
        Load();
    }

    public void CutsceneDone()
    {
        Cutscene_Data[] temp = Cutscene.cutsceneData;

        if (temp == null || temp.Length == 0)
        {
            Cutscene.cutsceneData = new Cutscene_Data[1];
            Cutscene.cutsceneData[0] = new Cutscene_Data(this);
            return;
        }

        Cutscene.cutsceneData = new Cutscene_Data[temp.Length + 1];

        for (int i = 0; i < temp.Length; i++)
        {
            Cutscene.cutsceneData[i] = temp[i];
            if (i == temp.Length - 1)
            {
                Cutscene.cutsceneData[i + 1] = new Cutscene_Data(this);
            }
        }
    }

}
[System.Serializable]
public class Cutscene_Data
{
    [SerializeField] public string id;
    public Cutscene_Data(Cutscene_Save cutscene)
    {
        id = cutscene.gameObject.name;
    }
}

