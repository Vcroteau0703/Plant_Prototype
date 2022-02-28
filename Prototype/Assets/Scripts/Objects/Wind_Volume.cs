using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Volume : Action_Volume
{
    public Collider col;

    public float intesity = 1.0f;
    public float power = 3.0f;
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
        Rigidbody rb = actor.GetComponent<Rigidbody>();
        float y = transform.InverseTransformPoint(col.bounds.center).y;
        Vector3 pos = transform.position + new Vector3(0, y, 0);
        Vector3 start = pos - (col.bounds.extents.y * Vector3.up);
        float y_diff = Mathf.Abs(Mathf.Clamp(actor.transform.position.y - start.y, 0.01f, float.PositiveInfinity));
        float force = Mathf.Pow(-Physics.gravity.y * intesity / y_diff * accel, power);
        rb.GetComponent<Rigidbody>().AddForce(force * transform.up, ForceMode.Force);
    }
}
