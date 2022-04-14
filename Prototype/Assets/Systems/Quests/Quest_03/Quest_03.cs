using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_03 : MonoBehaviour
{
    public Quest source;

    public void Start_Cave_Quest(Tablet_Tracker tablet_Tracker)
    {
        StartCoroutine(Cave_Quest(tablet_Tracker));
    }

    IEnumerator Cave_Quest(Tablet_Tracker tracker)
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        int tabletProg = tracker.tabletsRemaining - tracker.tablets;

        while (tabletProg < tasks[0].maxProgress)
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
