using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_02 : MonoBehaviour
{
    public Quest source;

    public int maxTermites;
    public int termitesLeft;

    public void Start_Termite_Quest()
    {
        termitesLeft = transform.GetComponentsInChildren<Termite>().Length;
        StartCoroutine(Termite_Quest());
    }

    IEnumerator Termite_Quest()
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        int temp = termitesLeft;
        while (source.Data.Current_Event.tasks[0].progress < tasks[0].maxProgress)
        {
            if(temp != termitesLeft)
            {
                Quest_System.Update_Task(tasks[0], 1);
                temp = termitesLeft;
            }
            yield return new WaitForEndOfFrame();
        }
        Quest_System.Next_Event(source);
    }


}
