using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class UI_Effects
{
    public static void Fade_Load(int scene)
    {
        GameObject fade_load = Resources.LoadAll<UI_Effects_Data>(Path.Combine("Data"))[0].fade_load;
        SaveSystem.Save(new Loading_Data(scene), "/Temp/Loading.data");
        GameObject go = Object.Instantiate(fade_load);
        go.name = "Fade_Load " + "(Level_" + scene.ToString("00") + ')';
    }
}