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
        //action 
    }

    public IEnumerator ActivateSap()
    {
        // make termites collectable and disable their hazard damage

        // set movement settings to slower ones
        yield return new WaitForSeconds(sapCooldown);
    }

}
