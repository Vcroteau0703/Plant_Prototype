using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablet_Tracker : MonoBehaviour
{
    public int tablets;
    public int tabletsRemaining = 4;

    // Start is called before the first frame update
    void Start()
    {
        tablets = tabletsRemaining;
    }

    public void UpdateTabletNum()
    {
        tablets -= 1;
    }
}
