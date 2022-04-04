using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public struct Event
{
    public string name;
    public bool completed;
    public List<Task> tasks;
    public UnityEvent OnCompletion;
}

