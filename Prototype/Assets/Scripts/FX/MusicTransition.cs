using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransition : MonoBehaviour
{
    AudioSource aS;
    float thisVol;
    public bool transition;
    internal bool isTransitioning;

    private void Awake()
    {
        aS = GetComponent<AudioSource>();
        thisVol = aS.volume;
        StartTransition(false);
    }

    public void StartTransition(bool exit)
    {
        if (transition)
        {
            if (exit)
            {
                StartCoroutine(VolumeTransition(thisVol, 0, 0.5f));
            }
            else
            {
                aS.volume = 0;
                StartCoroutine(VolumeTransition(0, thisVol, 0.5f));
            }

        }
    }
    
    IEnumerator VolumeTransition(float startVol, float finalVol, float cycleTime)
    {
        isTransitioning = true;
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            float currentVol = Mathf.Lerp(startVol, finalVol, t);
            aS.volume = currentVol;
            yield return null;
        }
        isTransitioning = false;
    }
}
