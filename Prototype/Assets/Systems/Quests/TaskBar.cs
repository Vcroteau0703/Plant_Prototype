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

    public List<Task> debug_list = new List<Task>();

    public void UpdateDisplay(Quest quest, Event targetEvent)
    {
        if (quest) { questTitle.text = quest.questTitle; }
        else { questTitle.text = ""; return; }

        if(targetEvent == null) {
            foreach(Task_UI t in taskContainer)
            {
                Destroy(t.gameObject);
            }
            return;
        }

        List<Task> targetTasks = targetEvent.tasks;
        debug_list = targetTasks;
        Task_UI[] currentTasks = taskContainer.GetComponentsInChildren<Task_UI>();

        for(int a = 0; a < currentTasks.Length; a++)
        {
            Task temp = Find_Task(currentTasks[a]); // Can the system find the current tasks in the UI in our current event

            if (temp != null)
            {
                Debug.Log("UPDATING TASK: " + currentTasks[a].name);
                Update_Task(currentTasks[a], temp.description, temp.progress, temp.maxProgress);
                if(temp.progress >= temp.maxProgress) { currentTasks[a].animator.SetTrigger("Fade"); }
            }
            else
            {
                Debug.Log("REMOVING TASK: " + currentTasks[a].name);
                currentTasks[a].animator.SetTrigger("Fade");
            }
        }

        for(int b = 0; b < targetTasks.Count; b++)
        {
            Task_UI temp = Find_TaskUI(targetTasks[b]); // Can the system find each taskUI in the target tasks in our current event

            if(temp == null && (targetTasks[b].progress < targetTasks[b].maxProgress || targetTasks[b].maxProgress == 0))
            {
                Debug.Log("ADDING TASK: " + targetTasks[b].description);
                AddTask(targetTasks[b].description, targetTasks[b].progress, targetTasks[b].maxProgress);
            }
        }
    }

    public Task_UI Find_TaskUI(Task target)
    {
        Task_UI[] currentTasks = taskContainer.GetComponentsInChildren<Task_UI>();

        foreach (Task_UI i in currentTasks)
        {
            if (i.name == target.description)
            {
                return i;
            }
        }
        return null;
    }

    public Task Find_Task(Task_UI target)
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;

        foreach (Task i in tasks)
        {
            if(i.description == target.name)
            {
                return i;
            }
        }
        return null;
    }

    public void Update_Task(Task_UI task, string description, int progress, int maxProgress)
    {
        Color progColor = progress >= maxProgress ? Color.green : Color.yellow;
        string text;
        if(maxProgress == 0){
            text = "";
        }
        else{
            text = progress.ToString() + "/" + maxProgress.ToString();           
        }      
        task.GetComponent<Task_UI>().SetProperties(description, text, Color.white, progColor);
    }

    public void AddTask(string description, int progress, int maxProgress)
    {
        Color progColor = progress >= maxProgress ? Color.green : Color.yellow;
        string text;
        if(maxProgress == 0){
            text = "";
        }
        else{
            text = progress.ToString() + "/" + maxProgress.ToString();           
        }      
        GameObject task = Instantiate(data.default_task, taskContainer);
        task.name = description;
        task.GetComponent<Task_UI>().SetProperties(description, text, Color.white, progColor);
    }
}
