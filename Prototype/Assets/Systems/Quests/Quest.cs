using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string questTitle;
    public List<Event> events;

    public List<string> Get_Event_Names()
    {
        List<string> result = new List<string>();

        foreach(Event e in events)
        {
            result.Add(e.name);
        }
        return result;
    }
}
