using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tablet : Interact
{
    private Quest_03 quest;

    new private void Awake()
    {
        base.Awake();
        quest = transform.GetComponentInParent<Quest_03>();
    }

    public void Pickup()
    {
        quest.tabletsRemaining -= 1;
    }
}
