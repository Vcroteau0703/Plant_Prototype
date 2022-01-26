using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Greenify : MonoBehaviour
{
    public Color grassStartColor;
    public Color grassEndColor;
    public Color groundStartColor;
    public Color groundEndColor;
    public Color waterStartColor;
    public Color waterEndColor;
    public float time;
    public Material grass;
    public Material water;
    public Material ground;
    public GameObject cutscene;
    public GameObject cutscene2;
    public GameObject[] waterObj;

    private void Awake()
    {
        grass.color = grassStartColor;
        ground.color = groundStartColor;
        water.color = waterStartColor;
    }

    [YarnCommand("change_world")]
    public void StartTheColorChange()
    {
        cutscene2.SetActive(true);
        StartCoroutine(ColorChange(grassStartColor, grassEndColor, time, grass));
        StartCoroutine(ColorChange(groundStartColor, groundEndColor, time, ground));
        StartCoroutine(ColorChange(waterStartColor, waterEndColor, time, water));
    }

    public void ActivateCutscene()
    {
        for(int i = 0; i < waterObj.Length; i++)
        {
            waterObj[i].tag = "Untagged";
        }
        cutscene.SetActive(true);
    }


    public IEnumerator ColorChange(Color startColor, Color endColor, float cycleTime, Material mat)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            mat.color = currentColor;
            yield return null;
        }
        ActivateCutscene();
        //StopAllCoroutines();
    }
}
