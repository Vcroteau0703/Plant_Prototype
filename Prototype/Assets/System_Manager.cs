using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_Manager : MonoBehaviour
{
    private void OnEnable()
    {
        SaveSystem.CurrentSave = "";
    }
}
