using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Quest_03 : MonoBehaviour
{
    public GameObject taskList;
    public Quest source;

    public int totalTablets;
    public int tabletsRemaining;

    public void Start_Cave_Quest()
    {
        tabletsRemaining = transform.GetComponentsInChildren<Tablet>().Length;
        StartCoroutine(Cave_Quest());
    }

    IEnumerator Cave_Quest()
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        int temp = tabletsRemaining;
        while (source.Data.Current_Event.tasks[0].progress < tasks[0].maxProgress)
        {
            if (temp != tabletsRemaining)
            {
                Quest_System.Update_Task(tasks[0], 1);
                temp = tabletsRemaining;
            }
            yield return new WaitForEndOfFrame();
        }
        Quest_System.Next_Event(source);
    }
}
