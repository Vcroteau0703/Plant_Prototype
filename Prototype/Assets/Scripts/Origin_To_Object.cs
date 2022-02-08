using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Origin_To_Object : MonoBehaviour
{
    public Transform reference_target;

    private void Update()
    {
        Quaternion rot = Quaternion.AngleAxis(reference_target.localEulerAngles.z % 180, Vector3.forward);
        Debug.Log(-reference_target.localEulerAngles.z % 180);
        Debug.DrawLine(transform.position, transform.position + (rot * Vector3.up), Color.blue);
    }
}
