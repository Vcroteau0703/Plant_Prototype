using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

public class Settings_UI : MonoBehaviour
{
    public Transform[] settingsContainers;

    public List<TMP_Dropdown> dropdowns;
    public List<Slider> sliders;

    private void Awake()
    {
        Settings.Initialize();

        dropdowns = new List<TMP_Dropdown>();
        sliders = new List<Slider>();
        foreach (Transform t in settingsContainers)
        {
            foreach (Transform b in t)
            {
                TMP_Dropdown dropdown = b.GetComponentInChildren<TMP_Dropdown>();
                Slider slider = b.GetComponentInChildren<Slider>();

                if (dropdown != null){dropdowns.Add(dropdown);}
                if (slider != null) { sliders.Add(slider); }
            }
        }

        foreach (TMP_Dropdown n in dropdowns)
        {
            n.onValueChanged.AddListener(delegate { Update_Setting(n.options[n.value].text, n.transform.parent.name); });
        }

        foreach(Slider s in sliders)
        {
            s.onValueChanged.AddListener(delegate { Update_Setting(s.value.ToString(), s.transform.parent.name); });
        }
    }

    private void OnEnable()
    {
        Refresh_Settings();
    }

    public void Refresh_Settings()
    {
        Setting_Data data = SaveSystem.Load<Setting_Data>("/Player/Settings.data");
        if(data == null) { Debug.Log("no data to refesh"); return; }

        string temp = "";

        foreach (TMP_Dropdown n in dropdowns)
        {
            switch (n.transform.parent.name) 
            {
                case "Display_Mode": Check_Change(data.display, n); continue;
                case "FPS": Check_Change(data.fps.ToString(), n); continue;
                case "V-Sync": temp = data.vSync == true ? "On" : "Off"; Check_Change(temp, n); continue;
                case "Shadow_Quality": Check_Change(data.shadow_Q, n); continue;
                case "Texture_Quality": Check_Change(data.texture_Q, n); continue;
                case "Bloom": temp = data.bloom == true ? "On" : "Off"; Check_Change(temp, n); continue;
            }
        }

        foreach (Slider s in sliders)
        {
            switch (s.transform.parent.name)
            {
                case "Brightness": Check_Change((data.brightness * 50).ToString(), s); continue;
                case "Master": Check_Change(data.masterVolume.ToString(), s); continue;
                case "Music": Check_Change(data.musicVolume.ToString(), s); continue;
                case "SFX": Check_Change(data.effects_Volume.ToString(), s); continue;
                case "Voice": Check_Change(data.voice_Volume.ToString(), s); continue;
            }
        }
    }
    private void Check_Change<T>(string targetSetting, T target)
    {
        if(target.GetType() == typeof(TMP_Dropdown))
        {
            TMP_Dropdown temp = target as TMP_Dropdown;
            int i = 0;
            foreach (TMP_Dropdown.OptionData a in temp.options)
            {
                Debug.Log(a.text + " | " + targetSetting);
                if (a.text == targetSetting) { temp.value = i; return; }
                i++;
            }
        }
        else if(target.GetType() == typeof(Slider))
        {
            Slider temp = target as Slider;
            temp.value = float.Parse(targetSetting);
        }
    }
    public void Update_Setting(string value, string setting)
    {
        Debug.Log(setting + " changed to " + value);
        switch (setting) 
        {
            case "Display_Mode":
                Settings.Display = value; break;
            case "FPS":
                Settings.FPS = int.Parse(value); break;
            case "V-Sync":
                Settings.V_Sync = value == "On" ? true : false; break;
            case "Shadow_Quality":
                Settings.Shadow_Quality = value; break;
            case "Texture_Quality":
                Settings.Texture_Quality = value; break;
            case "Brightness":
                Settings.Brightness = float.Parse(value); break;
            case "Bloom":
                Settings.Bloom = value == "On" ? true : false; break;
            case "Master":
                Settings.Master_Volume = float.Parse(value); break;
            case "Music":
                Settings.Music_Volume = float.Parse(value); break;
            case "SFX":
                Settings.Effects_Volume = float.Parse(value); break;
            case "Voice":
                Settings.Voice_Volume = float.Parse(value); break;

        }
    }

    public void Toggle_Slider_Navigation(Slider slider)
    {
        Navigation nav = slider.navigation;
        switch (nav.mode) 
        {
            case Navigation.Mode.Explicit:
                nav.mode = Navigation.Mode.None;
                break;
            case Navigation.Mode.None:
                nav.mode = Navigation.Mode.Explicit;
                break;      
        }
        slider.navigation = nav;
    }
    public void Enable_Slider_Naviation(Slider slider)
    {
        Navigation nav = slider.navigation;
        nav.mode = Navigation.Mode.Explicit;
        slider.navigation = nav;
    }
}

[System.Serializable]
public class Setting_Data
{
    public string display= "1920x1080", shadow_Q = "High", texture_Q = "High";
    public int fps = 60;
    public float brightness = 1.0f, masterVolume = -15, musicVolume = -15, effects_Volume = -15, voice_Volume = -15;
    public bool vSync = true, bloom = true;
    public Setting_Data(){}
}

public static class Settings
{
    private static Setting_Data Data = null;
    private static AudioMixer audioMixer;
    private static VolumeProfile[] profiles;
    private static void Save(){SaveSystem.Save(Data, "/Player/Settings.data");}
    public static void Initialize(){
        audioMixer = Resources.Load<AudioMixer>(Path.Combine("Data/Audio/MasterMixer"));
        profiles = Resources.LoadAll<VolumeProfile>(Path.Combine("Data", "Post-Processing"));

        if (Data == null) {
            Setting_Data temp = SaveSystem.Load<Setting_Data>("/Player/Settings.data");
            if(temp == null)
            {
                Data = new Setting_Data();
                return;
            }
            Data = temp;
            Display = Display;
            FPS = FPS;
            V_Sync = V_Sync;
            Shadow_Quality = Shadow_Quality;
            Texture_Quality = Texture_Quality;
            Bloom = Bloom;
            Brightness = Brightness;
            Master_Volume = Master_Volume;
            Music_Volume = Music_Volume;
            Effects_Volume = Effects_Volume;
            Voice_Volume = Voice_Volume;
        }
    }

    #region GRAPHICS
    public static string Display 
    { 
        get { 
            return Data.display;
        }
        set
        {
            Data.display = value;
            string[] nums = value.Split('x');
            int width = int.Parse(nums[0]);
            int height = int.Parse(nums[1]);
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
            Save();
        }
    }
    public static int FPS
    {
        get {
            return Data.fps;       
        }
        set
        {
            Data.fps = value;
            Application.targetFrameRate = value;
            Save();
        }
    }
    public static bool V_Sync 
    {
        get {
            return Data.vSync;
        }
        set
        {
            Data.vSync = value;
            if (value == true) { QualitySettings.vSyncCount = 1; }
            else { QualitySettings.vSyncCount = 0; }
            Save();
        }
    
    }
    public static string Shadow_Quality
    {
        get{
            return Data.shadow_Q;
        }
        set
        {
            Data.shadow_Q = value;
            UnityEngine.ShadowResolution res = UnityEngine.ShadowResolution.Medium;
            switch (value) 
            {
                case "Low": res = UnityEngine.ShadowResolution.Low; break;
                case "Medium": res = UnityEngine.ShadowResolution.Medium; break;
                case "High": res = UnityEngine.ShadowResolution.High; break;
                case "Maximum": res = UnityEngine.ShadowResolution.VeryHigh; break;
            }
            QualitySettings.shadowResolution = res;
            Save();
        }
    }
    public static string Texture_Quality
    {
        get{
            return Data.texture_Q;
        }
        set
        {
            Data.texture_Q = value;
            int quality = 0;
            switch (value)
            {
                case "Low": quality = 3; break;
                case "Medium": quality = 2; break;
                case "High": quality = 1; break;
                case "Maximum": quality = 0; break;
            }
            QualitySettings.masterTextureLimit = quality;
            Save();
        }
    }
    public static bool Bloom
    {
        get { return Data.bloom; }
        set
        {
            Data.bloom = value;
            foreach (VolumeProfile p in profiles)
            {
                if (p.TryGet(out Bloom a))
                {
                    a.active = value;
                }
            }
            Save();
        }
    }
    public static float Brightness
    {
        get { return Data.brightness * 50; }
        set
        {
            Data.brightness = value / 50;
            foreach (VolumeProfile p in profiles)
            {              
                if(p.TryGet(out LiftGammaGain a)){
                    a.gamma.value = new Vector4(a.gamma.value.x, a.gamma.value.y, a.gamma.value.z, -1.0f + (value / 50));
                }
            }
            Save();
        }
    }
    #endregion

    #region AUDIO
    public static float Master_Volume
    {
        get { return Data.masterVolume; }
        set { 
            Data.masterVolume = value;
            audioMixer.SetFloat("masterVol", value);
            Save();
        }
    }
    public static float Music_Volume
    {
        get { return Data.musicVolume; }
        set
        {
            Data.musicVolume = value;
            audioMixer.SetFloat("musicVol", value);
            Save();
        }
    }
    public static float Effects_Volume
    {
        get { return Data.effects_Volume; }
        set
        {
            Data.effects_Volume = value;
            audioMixer.SetFloat("sfxVol", value);
            Save();
        }
    }
    public static float Voice_Volume
    {
        get { return Data.voice_Volume; }
        set
        {
            Data.voice_Volume = value;
            audioMixer.SetFloat("vaVol", value);
            Save();
        }
    }
    #endregion
}
