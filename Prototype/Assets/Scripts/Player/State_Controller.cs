using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class State_Controller : MonoBehaviour, ISavable
{
    public bool isInvoking;
    public State[] states;
    public void Awake()
    {
        State_Controller_Data data = SaveSystem.Load<State_Controller_Data>("/Player/Abilities.data");
        if(data != null)
        {
            int i = 0;
            foreach(State_Data d in data.states_data)
            {
                states[i].active = d.active;
                states[i].enabled = d.enabled;
                states[i].name = d.name;
                states[i].looping = d.looping;
                i++;
            }
        }
    }
    public void Start()
    {
        Enable_State("Idle");
    }
    private void Update()
    {
        if (!isInvoking || Time.timeScale == 0) { return; }
        foreach (State s in states) {
            if (s.active && s.looping) {
                s.OnStateEnter.Invoke();
            }
            else if(s.active && !s.looping)
            {
                s.OnStateEnter.Invoke();
                isInvoking = false;
            }
        }
        //Debug.Log(Get_Active_State().name);
    }
    public State Get_Active_State()
    {
        foreach(State s in states)
        {
            if (s.active)
            {
                return s;
            }
        }
        return new State();
    }
    public void Disable_State(string name)
    {
        foreach (State s in states)
        {
            if (s.name == name)
            {
                s.enabled = false;
            }
        }
    }
    public void Enable_State(string name)
    {
        foreach (State s in states)
        {
            if (s.name == name)
            {
                s.enabled = true;
            }
        }
    }
    public void Request_State(string name)
    {
        State current = Get_Active_State();
        foreach (State s in states)
        {
            if (s.name == name && s.enabled && s != current)
            {
                current.active = false;
                current.OnStateExit.Invoke();
                s.active = true;
                isInvoking = true;
            }
        }
    }
    public State Get_State(string name)
    {
        foreach(State s in states)
        {
            if(s.name == name)
            {
                return s;
            }
        }
        return null;
    }

    public void Save()
    {
        State_Controller_Data data = new State_Controller_Data(states);
        SaveSystem.Save(data, "/Player/Abilities.data");
    }
}

[System.Serializable]
public class State_Controller_Data
{
    public State_Data[] states_data;

    public State_Controller_Data(State[] states)
    {
        states_data = new State_Data[states.Length];

        int i = 0;
        foreach(State s in states)
        {
            states_data[i] = new State_Data(s);
            i++;
        }
    }
}


[System.Serializable]
public class State_Data
{
    public string name;
    public bool enabled = true;
    public bool active;
    public bool looping = false;
    public State_Data(State state){
        this.name = state.name;
        this.enabled = state.enabled;
        this.active = state.active;
        this.looping = state.looping;
    }
}

[System.Serializable]
public class State 
{
    public string name;
    public bool enabled = true;
    public bool active;
    public bool looping = false;
    public UnityEvent OnStateEnter;
    public UnityEvent OnStateExit;
}

