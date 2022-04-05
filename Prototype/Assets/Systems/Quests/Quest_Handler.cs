using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest_Handler : MonoBehaviour, ISavable
{
    public TaskBar taskBar;
    public Quest _targetQuest;
    private Event currentEvent;
    public Quest[] allQuests;

    public List<Event_Action> actions;

    public static UnityEvent
        OnQuestStart,
        OnQuestComplete;
    public static UnityEvent
        OnEventStart,
        OnEventComplete;

    private void OnEnable()
    {
        OnQuestStart.AddListener(Quest_Complete);
        OnQuestComplete.AddListener(Quest_Complete);

        OnEventStart.AddListener(Event_Start);
        OnEventComplete.AddListener(Event_Complete);
    }

    public Quest Target_Quest{get{return _targetQuest;}
        set{
            _targetQuest = value;
            if (Application.isPlaying) { taskBar.UpdateDisplay(_targetQuest); }
        }
    }

    public void Save()
    {
        
    }
   
    public void Quest_Start()
    {
        taskBar.UpdateDisplay(Target_Quest);
        foreach(Event_Action a in actions)
        {
            if(a.originEvent == currentEvent.name)
            {
                a.Action.Invoke();
            }
        }
    }

    public void Quest_Complete()
    {

    }

    public void Event_Start()
    {

    }

    public void Event_Complete()
    {

    }

    private void OnValidate()
    {
        Target_Quest = _targetQuest;
    }
}

[System.Serializable]
public class Event_Action
{
    public string actionName;
    public string originEvent;
    public UnityEvent Action;
}
