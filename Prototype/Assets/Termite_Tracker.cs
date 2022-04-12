using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termite_Tracker : MonoBehaviour
{
    public int termites;
    internal int termitesRemaining = 18;

    private void Awake()
    {
        termites = transform.childCount;
    }

    internal void Update_Termite_Count()
    {
        termites -= 1;
    }
}
