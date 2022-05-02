using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermiteSFX : MonoBehaviour
{
    public AudioClip[] clips;
    int r;
    AudioSource aS;

    private void Awake()
    {
        aS = GetComponent<AudioSource>();
    }

    public void PlaySFX()
    {
        r = Random.Range(0, clips.Length);
        aS.clip = clips[r];
        aS.Play();
    }
}
