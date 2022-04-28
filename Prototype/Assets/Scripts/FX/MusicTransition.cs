using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicTransition : MonoBehaviour
{
    public AudioMixer master;

    private void Awake()
    {
        master.SetFloat("trueMasterVol", -80);
    }
    
    private void OnEnable()
    {
        StartTransition(false);
    }

    public void StartTransition(bool exit)
    {
        if (exit)
        {
            StartCoroutine(VolumeTransition(0, -80, 2f));
        }
        else
        {
            StartCoroutine(VolumeTransition(-80, 0, 2f));
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
            master.SetFloat("trueMasterVol", currentVol);
            yield return null;
        }
    }
}
