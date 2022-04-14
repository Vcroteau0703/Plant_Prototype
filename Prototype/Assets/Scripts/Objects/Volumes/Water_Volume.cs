using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Volume : Action_Volume
{
    public Collider col;

    private void Awake()
    {
        col = TryGetComponent(out Collider c) ? c : null;
    }

    private void OnEnable()
    {
        action = Deactivate_Sap;
    }

    public void Deactivate_Sap(GameObject actor)
    {
        Player player = actor.GetComponentInParent<Player>();
        player.inWater = true;
        if (player.sapActive)
        {
            player.SapEffectOff();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        player.inWater = false;
    }
}
