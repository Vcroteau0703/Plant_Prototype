using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string questTitle;
    public List<Event> events;
}

public class QuestData
{
    public string name;
    public bool isCompleted;
    public Event currentEvent;

    public QuestData(Quest quest, Event currentEvent, bool isCompleted)
    {
        name = quest.name;
        this.currentEvent = currentEvent;
        this.isCompleted = isCompleted;
    }
}
