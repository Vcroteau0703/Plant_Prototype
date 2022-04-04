using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Quest_System
{
    public static void Start_Quest(Quest quest)
    {
        Object.FindObjectOfType<TaskBar>().Target_Quest = quest;
    }

    public static void Complete_Event(Quest quest)
    {

    }

    public static void Complete_Quest(Quest quest)
    {

    }
}
