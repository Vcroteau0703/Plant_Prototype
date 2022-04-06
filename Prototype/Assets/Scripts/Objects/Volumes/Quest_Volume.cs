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
    }

    public override void OnTriggerExit(Collider other)
    {
        Quest_System.Complete_Quest(quest);
    }
}

