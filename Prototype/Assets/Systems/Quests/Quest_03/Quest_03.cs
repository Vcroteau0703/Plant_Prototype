using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Quest_03 : MonoBehaviour
{
    public GameObject taskList;
    public Quest source;
    public Tablet_Tracker tablet_Tracker;

    public void Start()
    {
        //if(source.Data.Current_Event != null)
        //{
        //    Quest_System.Start_Quest("Quest_03");
        //}
    }

    public void Start_Cave_Quest(Tablet_Tracker tablet_Tracker)
    {
        StartCoroutine(Cave_Quest(tablet_Tracker));
    }

    IEnumerator Cave_Quest(Tablet_Tracker tracker)
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        int tabletProg = tracker.tabletsRemaining - tracker.tablets;

        while (source.Data.Current_Event.tasks[0].progress < tasks[0].maxProgress)
        {
            if (tabletProg != tracker.tabletsRemaining - tracker.tablets)
            {
                tabletProg = tracker.tabletsRemaining - tracker.tablets;
                Quest_System.Update_Task(tasks[0], 1);
            }
            yield return new WaitForEndOfFrame();
        }
        Quest_System.Next_Event(source);
    }
}
