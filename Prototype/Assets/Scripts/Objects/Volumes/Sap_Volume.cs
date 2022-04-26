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
        Player player = actor.GetComponentInParent<Player>();
        if (player.sapActive)
        {
            StopAllCoroutines();
        }
        else
        {
            player.SapEffectOn();
            transform.parent.GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.parent.TryGetComponent<Player>(out Player player))
        {
            StartCoroutine(SapTimer(player));
        }
    }

    public IEnumerator SapTimer(Player player)
    {
        yield return new WaitForSeconds(sapCooldown);
        player.SapEffectOff();
        transform.parent.GetComponent<AudioSource>().Stop();
    }

}
