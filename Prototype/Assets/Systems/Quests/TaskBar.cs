using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TaskBar : MonoBehaviour
{
    
    [Header("Components:")]
    public TMP_Text questTitle;
    public Transform taskContainer;

    public TaskBarData data;

    private void Start()
    {
        data = TaskBarData.GetData(); 
    }

    public void UpdateDisplay(Quest quest)
    {
        foreach(Transform child in taskContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (quest) { questTitle.text = quest.questTitle; }
        else { questTitle.text = "No Quest Selected"; return; }

        foreach (Event e in quest.events)
        {
            foreach (Task t in e.tasks)
            {
                AddTask(t.description, t.progress);
            }
            break;
        }
    }

    public void AddTask(string description, string progress)
    {
        GameObject task = Instantiate(data.default_task, taskContainer);
        task.GetComponent<Task_UI>().SetProperties(description, progress);
    }
}
