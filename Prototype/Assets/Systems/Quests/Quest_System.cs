using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Quest_System
{
    /// <summary>
    /// Starts the quest from it's current progress point.
    /// </summary>
    /// <param name="quest"></param>

    public static void Start_Quest(Quest _quest)
    {
        if (_quest.Data.Completed) { return; }
        Quest_Handler.instance.Quest_Start(_quest);
    }

    [Yarn.Unity.YarnCommand("Start_Quest")]
    public static void Start_Quest(string _quest)
    {
        Quest target = Resources.Load<Quest>("Data/Quests/" + _quest);
        if (!target) { Debug.Log("Requested Quest Start parameter does not exist in Resources."); return; }

        if (target.Data.Completed) { return; }
        Quest_Handler.instance.Quest_Start(target);
    }

    public static void Start_Event(Quest quest, Event _event)
    {
        if(quest.Data.Current_Event.name == _event.name) { return; }      
        Quest_Handler.instance.Event_Start(_event);
    }

    public static void Complete_Event(Quest quest, Event _event)
    {       
        Quest_Handler.instance.Event_Complete(_event);
    }

    public static void Complete_Quest(Quest _quest)
    {
        Quest_Handler.instance.Quest_Complete(_quest);
    }

    public static void Update_Task(Task task, int progress)
    {
        Quest_Handler.instance.Update_Tasks(task, progress);
    }

    public static Event Get_Active_Event()
    {
        Event target = Quest_Handler.instance.Target_Event;
        if(target != null){return target;}
        else{return null;}     
    }

    public static void Next_Event(Quest quest)
    {
        Quest tQuest = Resources.Load<Quest>("Data/Quests/" + quest.name);
        if (!tQuest) { Debug.Log("Requested Quest Start parameter does not exist in Resources."); return; }

        if (tQuest.Data.Completed) { return; }

        Event curEvent = Quest_Handler.instance.Target_Event;

        Debug.Log("Current Event: " + curEvent);

        Event_Data[] eventData = tQuest.Data.Events;

        for (int i = 0; i < eventData.Length; i++)
        {
            if (curEvent != null && eventData[i].Name == curEvent.name)
            {
                Quest_Data data = tQuest.Data;
                data.Events[i].Completed = true;
                Quest_Handler.instance.Event_Complete(curEvent);
                tQuest.Data = data;
                continue;
            }
            else if (eventData[i].Completed == false)
            {
                curEvent = tQuest.Get_Event(eventData[i].Name);
                Quest_Handler.instance.Event_Start(curEvent);
                return;
            }
        }
        Debug.Log("No More Events to Load -> Quest Complete");
        Complete_Quest(tQuest);
    }

    [Yarn.Unity.YarnCommand("Next_Event")]
    public static void Next_Event(string quest)
    {
        Quest tQuest = Resources.Load<Quest>("Data/Quests/" + quest);
        if (!tQuest) { Debug.Log("Requested Quest Start parameter does not exist in Resources."); return; }

        if (tQuest.Data.Completed) { return; }

        Event curEvent = Quest_Handler.instance.Target_Event;

        Event_Data[] eventData = tQuest.Data.Events;

        for(int i = 0; i < eventData.Length; i++)
        {
            if(curEvent != null && eventData[i].Name == curEvent.name)
            {
                Quest_Data data = tQuest.Data;
                data.Events[i].Completed = true;
                Quest_Handler.instance.Event_Complete(curEvent);
                tQuest.Data = data;              
                continue;
            }
            else if(eventData[i].Completed == false)
            {
                curEvent = tQuest.Get_Event(eventData[i].Name);
                Quest_Handler.instance.Event_Start(curEvent);
                return;
            }
        }
        Debug.Log("No More Events to Load -> Quest Complete");
        Complete_Quest(tQuest);
    }
}
