using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sap_Volume : Action_Volume
{
    public int sapCooldown;

    public Collider col;

    private void Awake()
    {
        col = TryGetComponent(out Collider c) ? c : null;
    }

    private void OnEnable()
    {
        action = StartSap;
    }

    public void StartSap(GameObject actor)
    {
        Player player = actor.GetComponent<Player>();
        StartCoroutine(ActivateSap(player));
    }

    public IEnumerator ActivateSap(Player player)
    {
        if (player.sapActive)
        {
            yield return new WaitForSeconds(sapCooldown);
            player.SapEffectOff();
        }
        else
        {
            player.SapEffectOn();
            yield return new WaitForSeconds(sapCooldown);
            player.SapEffectOff();
        }
        
    }

}
