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
        if (quest.Data.Completed) { return; }
        Event target = Quest_Handler.instance.Target_Event;
        Event current = Quest_Handler.instance.Target_Event;
        if (target == null)
        {
            target = Quest_Handler.instance.Target_Quest.events[0];
            Quest_Handler.instance.Event_Start(target);
            return;
        }

        Event_Data[] eventData = quest.Data.Events;
        foreach(Event_Data d in eventData)
        {
            if (!d.Completed)
            {
                if(d.Name == current.name)
                {
                    d.Completed = true;
                    continue;
                }

                target = quest.Get_Event(d.Name);
                Quest_Handler.instance.Event_Start(target);
                return;
            }          
        }
        Complete_Quest(quest);
    }
}
