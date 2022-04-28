using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhispersSFX : MonoBehaviour
{
    public AudioClip[] audioClips;
    AudioSource a;
    public float audioTime;
    public float timer = 0.0f;
    float initVol;
    float endVol = 0f;
    int randomClip;

    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<AudioSource>();
        audioTime = a.clip.length;
        initVol = a.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > audioTime)
        {
            PlaySFX();
            StartCoroutine(FadeTransition(0.5f));
        }
        timer += Time.deltaTime;
    }

    void PlaySFX()
    {
        randomClip = Random.Range(0, audioClips.Length);
        a.clip = audioClips[randomClip];
        audioTime = a.clip.length;
        timer = 0.0f;
        a.Play();
    }

    IEnumerator FadeTransition(float cycleTime)
    {
        a.volume = 0.0f;
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            float currVol = Mathf.Lerp(a.volume, initVol, t);
            a.volume = currVol;
            yield return null;
        }
        yield return new WaitForSeconds(a.clip.length - (cycleTime*2));
        currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            float currVol = Mathf.Lerp(initVol, endVol, t);
            a.volume = currVol;
            yield return null;
        }
    }
}
