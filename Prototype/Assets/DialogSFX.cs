using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogSFX : MonoBehaviour
{
    public AudioClip[] spinnerClips;
    public float sPitch;
    public AudioClip[] groverClips;
    public float gPitch;
    public AudioClip[] frondClips;
    public float fPitch;
    public AudioClip[] aloeClips;
    public float aPitch;
    AudioSource aS;
    int r;

    private void Awake()
    {
        aS = GetComponent<AudioSource>();
    }

    [YarnCommand("PlayAudio")]
    public void PlayAudio(string charName)
    {
        switch (charName)
        {
            case "spinner":
                r = Random.Range(0, spinnerClips.Length);
                aS.clip = spinnerClips[r];
                aS.pitch = sPitch;
                aS.Play();
                break;
            case "grover":
                r = Random.Range(0, groverClips.Length);
                aS.clip = groverClips[r];
                aS.pitch = gPitch;
                aS.Play();
                break;
            case "frond":
                r = Random.Range(0, frondClips.Length);
                aS.clip = frondClips[r];
                aS.pitch = fPitch;
                aS.Play();
                break;
            case "aloe":
                r = Random.Range(0, aloeClips.Length);
                aS.clip = aloeClips[r];
                aS.pitch = aPitch;
                aS.Play();
                break;
        }
       
    }
}
