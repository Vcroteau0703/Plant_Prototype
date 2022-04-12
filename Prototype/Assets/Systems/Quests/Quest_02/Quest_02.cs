using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_02 : MonoBehaviour
{
    public Quest source;

    public void Start_Termite_Quest()
    {

    }

    public IEnumerator Termite_Quest()
    {
        List<Task> tasks = Quest_System.Get_Active_Event().tasks;
        
        Quest_System.Next_Event(source);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
