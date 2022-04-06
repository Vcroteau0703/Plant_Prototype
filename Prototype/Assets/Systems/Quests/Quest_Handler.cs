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

    public Quest_Data questData = null;

    public TaskBar taskBar;
    public Quest _targetQuest;

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

            if(value != null)
            {               
                taskBar.UpdateDisplay(_targetQuest, questData.Current_Event);
            }
            else
            {
                questData = null;
                taskBar.UpdateDisplay(_targetQuest, null);
            }          
        }
    }

    public Event Target_Event { 
        get {
            if (questData != null)
            {
                return questData.Current_Event;
            }
            else { return null; }
        }
        set{
            questData.Current_Event = value;
           
            if(Application.isPlaying) { taskBar.UpdateDisplay(_targetQuest, questData.Current_Event); }
            GameManager.SaveGame();
        }
    }

    public void Save()
    {
        if(questData == null) { return; }
        SaveSystem.Save(questData, "/Player/"+ questData.FileName + ".data");
    }

    public void Load_Quest(Quest quest)
    {
        if(quest == null) { return; }
        Quest_Data data = SaveSystem.Load<Quest_Data>("/Player/" + quest.name + ".data");
        if(data != null)
        {
            if (!data.Completed)
            {
                questData = data;
                Target_Quest = quest;
                Load_Event(questData);
                GameManager.SaveGame();
            }
            else
            {
                questData = null;
            }
            
        }
        else
        {
            questData = new Quest_Data(quest);
            Target_Quest = quest;
            Load_Event(questData);
            SaveSystem.Save(questData, "/Player/" + quest.name + ".data");
        }
    }

    public void Load_Event(Quest_Data data)
    {
        foreach(Event_Data d in data.Events)
        {
            if (!d.Completed)
            {
                foreach(Event e in Target_Quest.events)
                {
                    if(d.Name == e.name)
                    {
                        Target_Event = e;
                        return;
                    }
                }
            }
        }
    }

    public void Init()
    {
        Player_Data player = SaveSystem.Load<Player_Data>("/Player/Player.data");
        Quest quest = player != null ? Resources.Load<Quest>("Data/Quests/" + player.currentQuest) : null;
        if(player != null && quest != null) { Target_Quest = quest; } else {
            taskBar.UpdateDisplay(null, null);
            return;
        }
        questData = SaveSystem.Load<Quest_Data>("/Player/" + Target_Quest.name + ".data");
        Target_Event = questData.Current_Event;
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
        questData.Completed = true;
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

    public void Event_Start(string _event)
    {

    }
    public void Event_Complete(string _event)
    {

    }

    public void Update_Tasks(Task task, int progress)
    {
        foreach(Task t in questData.Current_Event.tasks)
        {
            if(t == task)
            {
                t.progress += progress;
            }
        }
        Save();
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
