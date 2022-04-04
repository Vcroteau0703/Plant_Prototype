using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControlsAppear : MonoBehaviour
{
    TextMeshProUGUI text;
    Image image;
    Color color = Color.white;
    public float fadeTime;

    private void Awake()
    {
        text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        image = transform.GetChild(0).GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(FadeIn(fadeTime));
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(FadeOut(fadeTime));
    }

    IEnumerator FadeIn(float cycleTime)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            color.a = Mathf.Lerp(0, 1, t);
            text.color = color;
            image.color = color;
            yield return null;
        }
    }
    IEnumerator FadeOut(float cycleTime)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            color.a = Mathf.Lerp(1, 0, t);
            text.color = color;
            image.color = color;
            yield return null;
        }
    }

}
