using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_Manager : MonoBehaviour
{
    private void Awake()
    {
        SaveSystem.CurrentSave = "";
    }

    private void Start()
    {
        Settings.Initialize();
    }
}
