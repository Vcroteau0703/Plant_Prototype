using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public Quest_Data _data;

    public Quest_Data Data{
        get{
            Quest_Data data = SaveSystem.Load<Quest_Data>("/Player/" + this.name + ".data");
            if (data == null)
            {
                data = new Quest_Data(this);
                SaveSystem.Save(data, "/Player/" + this.name + ".data");
            }
            return data;
        }
        set
        {
            _data = value;
            SaveSystem.Save(value, "/Player/" + this.name + ".data");
        }
    }

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

    public Event Get_Event(string name)
    {
        foreach(Event e in events)
        {
            if(e.name == name)
            {
                return e;
            }
        }
        return null;
    }
}
