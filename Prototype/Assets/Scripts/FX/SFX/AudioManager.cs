using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSettings[] audioSettings;

    public void SetExposedParam(int group)
    {
        //audioMixer.SetFloat(audioSettings[group].exposedParam, audioSettings[group].slider.value);
       //PlayerPrefs.SetFloat(audioSettings[group].exposedParam, audioSettings[group].slider.value);
    }

}
[System.Serializable]
public class AudioSettings
{
    public Slider slider;
    public string exposedParam;

    public AudioSettings()
    {
        slider.value = PlayerPrefs.GetFloat(exposedParam);
    }
}
