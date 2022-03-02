using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sap_Volume : Action_Volume
{
    public int sapCooldown;

    public Player_Control_Settings sapSettings;

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
        Player_Control_Settings originalSettings = player.gameObject.GetComponent<Player_Controller>().settings;
        // make termites collectable and disable their hazard damage
        player.sapActive = true;
        player.transform.GetChild(2).gameObject.SetActive(true);
        player.gameObject.GetComponent<Player_Controller>().settings = sapSettings;
        // set slow movement settings

        yield return new WaitForSeconds(sapCooldown);

        player.sapActive = false;
        player.transform.GetChild(2).gameObject.SetActive(false);
        player.gameObject.GetComponent<Player_Controller>().settings = originalSettings;
    }

}
