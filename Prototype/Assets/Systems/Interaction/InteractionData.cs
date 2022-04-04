using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Interaction Data",menuName = "Interaction Data")]
public class InteractionData : ScriptableObject
{
    public GameObject defaultInteractionText;
    public GameObject defaultTextCanvas;
    public static InteractionData Get(){return Resources.Load<InteractionData>("Data/Interaction Data");}
}

