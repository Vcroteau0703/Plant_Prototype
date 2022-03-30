using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string pickupID;
    public bool destroy = false;

    private void Awake()
    {
        //Register(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            player.currColl++;
            destroy = true;
            gameObject.SetActive(false);
        }
    }

    //public void Save()
    //{
    //    PickupData data = SaveSystem.Load<PickupData>(pickupID);

    //    if (data == null && destroy)
    //    {
    //        data = new PickupData();
    //        data.isDestroyed = true;
    //        SaveSystem.Save<PickupData>(data, pickupID);
    //    }

    //}

    //public void Load()
    //{
    //    PickupData data = SaveSystem.Load<PickupData>(pickupID);

    //    if (data != null && data.isDestroyed)
    //    {
    //        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    //        player.currColl++;
    //        Destroy(gameObject);
    //    }
    //}
}
