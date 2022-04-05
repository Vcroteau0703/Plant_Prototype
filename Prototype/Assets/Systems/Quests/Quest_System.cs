using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Quest_System
{
    /// <summary>
    /// Starts the quest from it's current progress point.
    /// </summary>
    /// <param name="quest"></param>
    public static void Start_Quest(Quest quest)
    {
        Quest_Handler.OnQuestStart.Invoke();       
    }

    public static void Start_Event()
    {
        Quest_Handler.OnEventStart.Invoke();
    }

    public static void Complete_Event()
    {
        Quest_Handler.OnEventComplete.Invoke();
    }

    public static void Complete_Quest()
    {
        Quest_Handler.OnQuestComplete.Invoke();
    }
}
