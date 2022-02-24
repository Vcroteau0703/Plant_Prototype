using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Volume : Action_Volume
{
    public float power = 1.0f;
    public float accel = 1.0f;

    private void OnEnable()
    {
        action = Wind_Force;
    }

    public void Wind_Force(GameObject actor)
    {
        float y_diff = Mathf.Clamp(actor.transform.position.y - transform.position.y, 0.01f, float.PositiveInfinity);
        float force = Mathf.Pow(-Physics.gravity.y *  1 / y_diff * accel, power);
        Debug.Log(force);
        actor.GetComponent<Rigidbody>().AddForce(force * Vector2.up, ForceMode.Acceleration);
    }
}
