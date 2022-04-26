using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour, ISavable
{
    public static Cutscene_Data[] cutsceneData;

    private void Awake()
    {
        Cutscene_Save_Data data = SaveSystem.Load<Cutscene_Save_Data>("/Levels/Cutscene_Save.data");
        if(data != null)
        {
            cutsceneData = data.cutscenes;
        }
    }

    public void Save()
    {
        Cutscene_Save_Data data = new Cutscene_Save_Data();
        SaveSystem.Save(data, "/Levels/Cutscene_Save.data");
    }
}
[System.Serializable]
public class Cutscene_Save_Data
{
    public Cutscene_Data[] cutscenes;
    public Cutscene_Save_Data()
    {
        cutscenes = Cutscene.cutsceneData;
    }
          
}
