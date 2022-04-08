using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Volume : Action_Volume
{
    public Quest quest;

    private void OnEnable()
    {
        action = Activate;
    }

    public void Activate(GameObject actor)
    {
        Quest_System.Start_Quest(quest);
        Quest_System.Start_Event(quest, quest.events[0]);
    }

    public override void OnTriggerExit(Collider other)
    {
        if ((Layer_Filter & (1 << other.gameObject.layer)) == 0) { return; }
        Debug.Log(Quest_System.Get_Active_Event());
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        Quest_System.Update_Task(tasks[0], 1);
        //Quest_System.Complete_Quest(quest);
    }
}

