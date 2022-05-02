using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_02 : MonoBehaviour
{
    public Quest source;

    public void Start_Termite_Quest(Termite_Tracker termiteTracker)
    {
        StartCoroutine(Termite_Quest(termiteTracker));
    }

    IEnumerator Termite_Quest(Termite_Tracker tracker)
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        int termiteProg = tracker.termitesRemaining - tracker.termites;

        while (source.Data.Current_Event.tasks[0].progress < tasks[0].maxProgress)
        {
            if(termiteProg != tracker.termitesRemaining - tracker.termites)
            {
                termiteProg = tracker.termitesRemaining - tracker.termites;
                Quest_System.Update_Task(tasks[0], 1);
            }
            yield return new WaitForEndOfFrame();
        }
        Quest_System.Next_Event(source);
    }


}
