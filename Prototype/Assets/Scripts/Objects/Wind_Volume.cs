using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Volume : Action_Volume
{
    public Collider col;

    public float power = 1.0f;
    public float accel = 1.0f;

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
        Vector3 start = transform.position - (col.bounds.extents.y * Vector3.up);
        float y_diff = Mathf.Clamp(actor.transform.position.y - start.y, 0.01f, float.PositiveInfinity);
        float force = Mathf.Pow(-Physics.gravity.y *  3 / y_diff * accel, power);
        Debug.Log(force);
        actor.GetComponent<Rigidbody>().AddForce(force * Vector2.up, ForceMode.Acceleration);
    }
}
