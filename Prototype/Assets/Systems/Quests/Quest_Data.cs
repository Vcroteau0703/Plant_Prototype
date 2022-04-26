using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest_Data
{
    public string FileName;
    public string Name;
    public bool Completed;
    public Event Current_Event;
    public Event_Data[] Events;
    public Cutscene_Data[] cutscenes;

    public Quest_Data(Quest quest)
    {
        FileName = quest.name;
        Name = quest.questTitle;
        Events = Get_Events(quest);
    }

    Event_Data[] Get_Events(Quest quest)
    {
        Event_Data[] temp = new Event_Data[quest.events.Count];
        for(int i = 0; i < temp.Length; i++)
        {
            temp[i] = new Event_Data(quest.events[i]);
        }
        return temp;
    }
}
[System.Serializable]
public class Event_Data
{
    public string Name;
    public bool Completed;
    public Task_Data[] Tasks;

    public Event_Data(Event _event)
    {
        Name = _event.name;
        Tasks = Get_Tasks(_event);
    }

    Task_Data[] Get_Tasks(Event _event)
    {
        Task_Data[] temp = new Task_Data[_event.tasks.Count];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = new Task_Data(_event.tasks[i].progress);
        }
        return temp;
    }
}
[System.Serializable]
public class Task_Data
{
    public int Progress;
    public Task_Data(int progress)
    {
        Progress = progress;
    }
}
