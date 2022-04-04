using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TaskBar : MonoBehaviour
{
    public Quest _targetQuest;
    public Quest Target_Quest{
        get{
            return _targetQuest;
        }
        set{
            _targetQuest = value;
            if (Application.isPlaying) { UpdateDisplay(); }        
        }
    }
   
    [Header("Components:")]
    public TMP_Text questTitle;
    public Transform taskContainer;

    public TaskBarData data;

    private void Start()
    {
        data = TaskBarData.GetData(); 
    }

    public void UpdateDisplay()
    {
        foreach(Transform child in taskContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (Target_Quest) { questTitle.text = Target_Quest.questTitle; }
        else { questTitle.text = "No Quest Selected"; return; }

        foreach (Event e in Target_Quest.events)
        {
            if (!e.completed)
            {
                foreach (Task t in e.tasks)
                {
                    AddTask(t.description, t.progress);
                }
                break;
            }
        }
    }

    public void AddTask(string description, string progress)
    {
        GameObject task = Instantiate(data.default_task, taskContainer);
        task.GetComponent<Task_UI>().SetProperties(description, progress);
    }
    private void OnValidate()
    {
        Target_Quest = _targetQuest;
    }

}
