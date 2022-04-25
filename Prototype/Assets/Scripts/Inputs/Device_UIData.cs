using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Device_UIData", menuName = "Device_UIData")]
public class Device_UIData : ScriptableObject
{
    public Device_UIElements[] device_UIElements;
    public static Device_UIData Get() { return Resources.Load<Device_UIData>("Data/Device_UIData"); }
    public Device_UIElements GetDeviceUI(string target_platform)
    {
        foreach (Device_UIElements j in device_UIElements)
        {
            int i = 0;
            foreach(char c in j.deviceName)
            {
                if(j.deviceName.Length-1 == i)
                {
                    return j;
                }
                else if(target_platform[i] == c)
                {
                    i++;
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
        Debug.Log("Target Platform Not Found! -> Changed to Default (Keyboard)");
        return device_UIElements[0];
    }
}

[System.Serializable]
public class Device_UIElements
{
    public string deviceName;
    public Sprite[] binding;
}
