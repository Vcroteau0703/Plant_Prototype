using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings_UI : MonoBehaviour
{
    public Transform[] settingsContainers;

    public List<TMP_Dropdown> dropdowns;
    public List<Slider> sliders;

    public GameObject first_Select;

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
        StartCoroutine(Set_First_Selected(first_Select));
    }

    public IEnumerator Set_First_Selected(GameObject a)
    {
        yield return new WaitForEndOfFrame();
        if(a.TryGetComponent<Button>(out Button b))
        {
            b.Select();
        }
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
                case "Bloom": temp = data.vSync == true ? "On" : "Off"; Check_Change(temp, n); continue;
            }
        }

        foreach (Slider s in sliders)
        {
            switch (s.transform.parent.name)
            {
                case "Brightness": Check_Change(data.brightness.ToString(), s); continue;
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
        }
    }

    public void Disable_Slider_Naviation(Slider slider)
    {
        Navigation nav = slider.navigation;     
        nav.mode = Navigation.Mode.None;
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
    public string display, shadow_Q, texture_Q;
    public int fps;
    public float brightness;
    public bool vSync, bloom;
    public Setting_Data(){}
}

public static class Settings
{
    private static Setting_Data Data = null;

    private static void Save(){SaveSystem.Save(Data, "/Player/Settings.data");}

    public static void Initialize(){
        if (Data == null) {
            Setting_Data temp = SaveSystem.Load<Setting_Data>("/Player/Settings.data");
            if(temp == null)
            {
                Data = new Setting_Data();
                return;
            }
            Data = temp;         
        }
    }

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
            ShadowResolution res = ShadowResolution.Medium;
            switch (value) 
            {
                case "Low": res = ShadowResolution.Low; break;
                case "Medium": res = ShadowResolution.Medium; break;
                case "High": res = ShadowResolution.High; break;
                case "Maximum": res = ShadowResolution.VeryHigh; break;
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
    public static float Brightness
    {
        get { return Data.brightness; }
        set
        {
            Data.brightness = value;
            Screen.brightness = value;
            Save();
        }
    } 
}
