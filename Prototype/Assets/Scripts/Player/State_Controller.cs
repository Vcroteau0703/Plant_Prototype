using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class State_Controller : MonoBehaviour
{
    public bool isInvoking;
    public List<State> states;
    public void Start()
    {
        Enable_State("Idle");
    }
    private void Update()
    {
        if (!isInvoking) { return; }
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

