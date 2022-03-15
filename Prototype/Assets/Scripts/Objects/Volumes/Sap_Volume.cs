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
        if (player.sapActive)
        {
            StopAllCoroutines();
        }
        else
        {
            player.SapEffectOn();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            StartCoroutine(SapTimer(player));
        }
    }

    public IEnumerator SapTimer(Player player)
    {
        yield return new WaitForSeconds(sapCooldown);
        player.SapEffectOff();
    }

}
