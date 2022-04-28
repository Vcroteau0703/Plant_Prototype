using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicTransition : MonoBehaviour
{
    public AudioMixer master;
    float thisVol;

    private void Awake()
    {
        master.GetFloat("masterVol", out thisVol);
        StartTransition(false);
    }

    public void StartTransition(bool exit)
    {
        if (exit)
        {
            StartCoroutine(VolumeTransition(thisVol, -80, 2f));
        }
        else
        {
            master.SetFloat("masterVol", -80);
            StartCoroutine(VolumeTransition(-80, thisVol, 2f));
        }
    }
    
    IEnumerator VolumeTransition(float startVol, float finalVol, float cycleTime)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            float currentVol = Mathf.Lerp(startVol, finalVol, t);
            master.SetFloat("masterVol", currentVol);
            yield return null;
        }
    }
}
