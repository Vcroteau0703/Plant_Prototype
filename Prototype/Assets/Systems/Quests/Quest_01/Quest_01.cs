using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_01 : MonoBehaviour
{
    public Quest source;

    public bool cling = false, slide = false, jump = false;


    public void Start_Cling_Tutorial(State_Controller controller)
    {
        StartCoroutine(Cling_Tutorial(controller));
    }

    public IEnumerator Cling_Tutorial(State_Controller player)
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;

        while (!cling || !slide || !jump)
        {          
            switch (player.Get_Active_State().name)
            {
                case "Cling":
                    if (cling) { break; }
                    cling = true;
                    Quest_System.Update_Task(tasks[0], 1);
                    break;

                case "Slide":
                    if (slide) { break; }
                    slide = true;
                    Quest_System.Update_Task(tasks[1], 1);
                    break;

                case "Wall Jump":
                    if (jump) { break; }
                    Quest_System.Update_Task(tasks[2], 1);
                    jump = true;
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
        Quest_System.Next_Event(source);
    }
}
