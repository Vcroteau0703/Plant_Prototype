using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;

[System.Serializable]
public class Event_Actions : ISerializationCallbackReceiver
{
    public Quest quest;
    public static List<string> eventsTemp;
    public static List<string> events;
    [ListToPopup(typeof(Event_Actions), "eventsTemp")]
    public string Event;

    public UnityEvent OnEventStart, OnEventComplete;
    public void OnBeforeSerialize()
    {
        if (quest != null)
        {
            events = quest.Get_Event_Names();
            eventsTemp = events;
        }
        else
        {
            events = new List<string> { "<None>" };
            eventsTemp = events;
        }
    }

    public void OnAfterDeserialize() { }
}

[System.Serializable]
public class Quest_Actions
{
    public Quest Quest;
    public UnityEvent OnQuestStart, OnQuestComplete;
}


public class Quest_Handler : MonoBehaviour, ISavable
{
    public static Quest_Handler instance;

    public TaskBar taskBar;
    public Quest _targetQuest;
    public Event _targetEvent;

    public Quest[] allQuests;

    //public List<Event_Action> actions;

    public List<Event_Actions> Event_Triggers;
    public List<Quest_Actions> Quest_Triggers;
        

    private void OnEnable()
    {
        instance = this;       
    }

    private void Start()
    {
        Init();       
    }

    public Quest Target_Quest{get{return _targetQuest;}
        set{                      
            _targetQuest = value;

            if(value == null)
            {
                taskBar.UpdateDisplay(null, null);
                return;
            }
            taskBar.UpdateDisplay(value, value.Data.Current_Event);
        }
    }

    public Event Target_Event { 
        get {
            if (Target_Quest != null)
            {
                return Target_Quest.Data.Current_Event;
            }
            else { return null; }
        }
        set{
            _targetEvent = value;
            Quest_Data data = Target_Quest.Data;
            data.Current_Event = value;
            Target_Quest.Data = data;
            Debug.Log(Target_Quest.Data.Current_Event.name);
            taskBar.UpdateDisplay(Target_Quest, value);
            GameManager.SaveGame();
        }
    }

    public void Save()
    {
        if(Target_Quest.Data == null) { return; }
        SaveSystem.Save(Target_Quest.Data, "/Player/"+ Target_Quest.Data.FileName + ".data");
    }

    public void Load_Quest(Quest quest)
    {
        if(quest == null || quest.Data.Completed) { return; }

        Target_Quest = quest;
        GameManager.SaveGame();    
    }

    public void Load_Event(Event _event)
    {
        if(Target_Quest == null) { return; }

        Event temp = Target_Quest.Get_Event(_event.name);

        Target_Event = temp;     
    }

    public void Init()
    {
        Player_Data player = SaveSystem.Load<Player_Data>("/Player/Player.data");
        Quest quest = player != null ? Resources.Load<Quest>("Data/Quests/" + player.currentQuest) : null;

        if(player == null || quest == null) { return; }

        Target_Quest = quest;
        Target_Event = quest.Data.Current_Event;
    }


    public void Quest_Start(Quest quest)
    {
        Load_Quest(quest);      
        if(Target_Quest != quest){return;}
        AddQuest(quest);
        
        foreach (Quest_Actions a in Quest_Triggers)
        {
            if(a.Quest == quest)
            {
                a.OnQuestStart.Invoke();
            }
        }

        taskBar.UpdateDisplay(Target_Quest, Target_Event);       
    }
    public void Quest_Complete(Quest quest)
    {
        if(Target_Quest != quest){return;}
        Quest_Data data = Target_Quest.Data;
        data.Completed = true;
        Target_Quest.Data = data;
        GameManager.SaveGame();
        Target_Quest = null;
        RemoveQuest(quest);
        GameManager.SaveGame();

        foreach (Quest_Actions a in Quest_Triggers)
        {
            if (a.Quest == quest)
            {
                a.OnQuestComplete.Invoke();
            }
        }

        taskBar.UpdateDisplay(Target_Quest, Target_Event);
    }

    public void Event_Start(Event _event)
    {
        foreach (Event_Actions a in Event_Triggers)
        {
            if (a.Event == _event.name)
            {
                a.OnEventStart.Invoke();
            }
        }

        Load_Event(_event);
    }
    public void Event_Complete(Event _event)
    {
        foreach (Event_Actions a in Event_Triggers)
        {
            if (a.Event == _event.name)
            {
                a.OnEventComplete.Invoke();
            }
        }
    }

    public void Update_Tasks(Task task, int progress)
    {
        Quest_Data data = Target_Quest.Data;

        foreach (Task t in data.Current_Event.tasks)
        {
            if(t.description == task.description)
            {
                t.progress = t.progress + progress >= t.maxProgress ? t.maxProgress : t.progress + progress;
                Debug.Log("Updating Task...");
                Target_Quest.Data = data;
            }
        }
        taskBar.UpdateDisplay(Target_Quest, Target_Event);
    }

    public Quest GetNextQuest()
    {
        if(Target_Quest == null && allQuests[0] != null) {return allQuests[0];}
        
        for(int i = 0; i < allQuests.Length; i++)
        {
            if(i == allQuests.Length - 1) { return allQuests[0]; }
            else if(allQuests[i] == Target_Quest)
            {
                return allQuests[i + 1];
            }
        }
        return null;
    }

    public void AddQuest(Quest quest)
    {      
        Quest[] temp = new Quest[allQuests.Length + 1];

        foreach(Quest q in allQuests)
        {
            if(q == quest){
                Debug.Log("Quest Exists"); 
                return;
            }
        }

        for(int i = 0; i < temp.Length; i++)
        {
            if(allQuests.Length == 0) { temp[0] = quest; break; }
            else if (i == allQuests.Length - 1) { temp[i+1] = quest; }
            else { temp[i] = allQuests[i]; }
        }
        allQuests = temp;     
    }

    void RemoveQuest(Quest quest)
    {
        if(allQuests.Length - 1 == 0)
        {
            allQuests = new Quest[0];
            return;
        }

        Quest[] temp = new Quest[allQuests.Length-1];

        for (int i = 0; i < temp.Length; i++)
        {
            if (quest == allQuests[i]) 
            {
                for(int k = i; k < temp.Length; k++)
                {
                    temp[k] = allQuests[k + 1];
                }
                break;
            }
            temp[i] = allQuests[i];
        }
        allQuests = temp;
    }  
}
