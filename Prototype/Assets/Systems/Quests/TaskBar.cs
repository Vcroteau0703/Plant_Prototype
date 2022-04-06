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

    public void UpdateDisplay(Quest quest, Event targetEvent)
    {
        foreach (Transform child in taskContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (quest) { questTitle.text = quest.questTitle; }
        else { questTitle.text = ""; return; }

        if (targetEvent != null)
        {
            foreach (Task t in targetEvent.tasks)
            {
                AddTask(t.description, t.progress, t.maxProgress);
            }
        }      
    }

    public void AddTask(string description, int progress, int maxProgress)
    {
        string text;
        if(maxProgress == 0){
            text = "";
        }
        else{
            text = progress < maxProgress ? progress.ToString() + "/" + maxProgress.ToString() : "Complete";
        }      
        GameObject task = Instantiate(data.default_task, taskContainer);
        task.GetComponent<Task_UI>().SetProperties(description, text);
    }
}
