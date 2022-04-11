using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "New TaskBarData", menuName = "Task Bar Data")]
public class TaskBarData : ScriptableObject
{
    public GameObject default_task;

    public static TaskBarData GetData()
    {
        TaskBarData[] data = Resources.LoadAll<TaskBarData>("Data");
        if (!data[0]) { Debug.Log("Missing Data for Task Bar in Data folder"); return null;  }
        return data[0];
    }
}
