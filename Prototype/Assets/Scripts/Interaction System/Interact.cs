using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Interact : MonoBehaviour
{
    public UnityEvent interactFunctions;

    public void TriggerEvent()
    {
        interactFunctions.Invoke();
    }
}


