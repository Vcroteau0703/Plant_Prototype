using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Data : MonoBehaviour
{
    Pickup[] all;

    void Register(Pickup p)
    {
        Pickup[] temp = new Pickup[all.Length + 1];
        for(int i = 0; i < all.Length; i++)
        {
            temp[i] = all[i];
        }
        temp[all.Length] = p;
        all = temp;
    }

}
