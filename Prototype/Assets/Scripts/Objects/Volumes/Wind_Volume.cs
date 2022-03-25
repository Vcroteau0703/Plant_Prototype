using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Volume : Action_Volume
{
    public Collider col;

    public float power = 3.0f;

    private void Awake()
    {
        col = TryGetComponent(out Collider c) ? c : null;
    }

    private void OnEnable()
    {
        action = Wind_Force;
    }

    public void Wind_Force(GameObject actor)
    {
        Player_Controller p = actor.GetComponentInParent<Player_Controller>();
        if (p != null){
            if (p.state_controller.Get_Active_State().name != "Gliding") { return; }
        }
        Rigidbody rb = p.rb;      
        float force = rb.velocity.magnitude > power ? 0 : power;
        rb.GetComponent<Rigidbody>().AddForce(force * transform.up, ForceMode.Force);
    }
}
