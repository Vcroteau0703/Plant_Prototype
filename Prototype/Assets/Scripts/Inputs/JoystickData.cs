using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Joystick Data", menuName = "Joystick Data")]
public class JoystickData : ScriptableObject
{
    public Joystick_UIElements[] JoystickUI;
    public static JoystickData Get() { return Resources.Load<JoystickData>("Data/Joystick Data"); }
    public Joystick_UIElements GetJoystickUI(string target_platform)
    {
        foreach (Joystick_UIElements j in JoystickUI)
        {
            if (j.joystickName == target_platform)
            {
                return j;
            }
        }
        Debug.Log("Target Platform Not Found!");
        return new Joystick_UIElements(null, null);
    }
}

[System.Serializable]
public struct Joystick_UIElements
{
    public string joystickName;
    public Sprite interact;

    public Joystick_UIElements(string name, Sprite interact)
    {
        this.joystickName = name;
        this.interact = interact;
    }
}
