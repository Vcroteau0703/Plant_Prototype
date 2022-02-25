using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.Playables;

public class Greenify : MonoBehaviour
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

    private void Awake()
    {
        water.SetColor("_Emission", waterStartEmission);
        water.SetColor("_BaseColor", waterStartColor);

        //StartTheColorChange();
    }

    [YarnCommand("change_world")]
    public void StartTheColorChange()
    {
        StartCoroutine(EmissionChange(waterStartEmission, waterEndEmission, time, water));
        StartCoroutine(ColorChange(waterStartColor, waterEndColor, time, water));
        ActivateCutscene();
    }

    public void ActivateCutscene()
    {
        for(int i = 0; i < hazards.Length; i++)
        {
            hazards[i].GetComponent<Hazard>().ChangeDamage(0);
        }
        for (int i = 0; i < hazards.Length; i++)
        {
            hazards[i].GetComponent<MeshCollider>().convex = true;
            hazards[i].GetComponent<MeshCollider>().isTrigger = true;
        }
        cutscene.Play();
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
}
