using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Data : MonoBehaviour, ISavable
{
    Pickup[] all;

    public void Save()
    {
        Pickup[] data = SaveSystem.Load<Pickup[]>("/Pickups.data");

        if (data == null)
        {
            data = all;
            SaveSystem.Save<Pickup[]>(data, "/Pickups.data");
        }
    }

    public void Load()
    {
        Pickup[] data = SaveSystem.Load<Pickup[]>("/Pickups.data");

        if (data != null)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (data[i].destroy)
                {
                    Destroy(all[i].gameObject);
                    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    player.currColl++;
                }
            }
        }
    }

    internal void Register(Pickup p)
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
