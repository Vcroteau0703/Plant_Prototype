using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.Playables;

public class Greenify : MonoBehaviour, ISavable
{
    [ColorUsage(true, true)]
    public Color waterStartEmission;
    [ColorUsage(true, true)]
    public Color waterEndEmission;
    public Color waterStartColor;
    public Color waterEndColor;
    public float time;
    //public Material grass;
    public Material water;
   // public Material ground;
    public PlayableDirector cutscene;
    //public GameObject cutscene2;
    public GameObject[] hazards;
    public bool greenDone = false;

    private void Awake()
    {
        Load();
    }

    [YarnCommand("change_world")]
    public void StartTheColorChange()
    {
        StartCoroutine(EmissionChange(waterStartEmission, waterEndEmission, time, water));
        StartCoroutine(ColorChange(waterStartColor, waterEndColor, time, water));
        ActivateCutscene();
    }

    public void CutsceneFinished()
    {
        cutscene.Pause();
    }

    public void ActivateCutscene()
    {
        for(int i = 0; i < hazards.Length; i++)
        {
            hazards[i].GetComponent<Hazard>().ChangeDamage(0);
            hazards[i].GetComponent<Water_Volume>().enabled = true;
        }
        cutscene.Play();
        greenDone = true;
    }


    public IEnumerator EmissionChange(Color startColor, Color endColor, float cycleTime, Material mat)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            mat.SetColor("_Emission", currentColor);
            yield return null;
        }
    }

    public IEnumerator ColorChange(Color startColor, Color endColor, float cycleTime, Material mat)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            water.SetColor("_BaseColor", currentColor);
            yield return null;
        }
        //StopAllCoroutines();
    }

    public void Save()
    {
        GreenifyData data = SaveSystem.Load<GreenifyData>("/Level01_green.data");

        if (data == null && greenDone)
        {
            data = new GreenifyData();
            data.isDone = true;
            SaveSystem.Save<GreenifyData>(data, "/Level01_green.data");
        }

        //throw new System.NotImplementedException();
    }
    public void Load()
    {
        GreenifyData data = SaveSystem.Load<GreenifyData>("/Level01_green.data");

        if (data != null && data.isDone)
        {
            Debug.Log(data.isDone);
            water.SetColor("_Emission", waterEndEmission);
            water.SetColor("_BaseColor", waterEndColor);
            cutscene.initialTime = 1137f;
            for (int i = 0; i < hazards.Length; i++)
            {
                hazards[i].GetComponent<Hazard>().ChangeDamage(0);
                hazards[i].GetComponent<Water_Volume>().enabled = true;
            }
            cutscene.Play();
            cutscene.Pause();
        }
        else
        {
            water.SetColor("_Emission", waterStartEmission);
            water.SetColor("_BaseColor", waterStartColor);

        }
    }


}
[System.Serializable]
public class GreenifyData
{
    public bool isDone = false;

}
