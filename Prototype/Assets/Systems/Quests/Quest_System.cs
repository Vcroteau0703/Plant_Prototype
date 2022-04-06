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
        Quest_Handler.instance.Quest_Start(_quest);
    }

    public static void Start_Event(string _event)
    {
        Quest_Handler.instance.Event_Start(_event);
    }

    public static void Complete_Event(string _event)
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
}
