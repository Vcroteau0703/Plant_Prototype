using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTransform : MonoBehaviour
{
    private void Awake()
    {
        
    }
    public void ChangePos(float x, float y, float z)
    {
        Vector3 pos = new Vector3(x, y, z);
        transform.position = pos;
    }

    public void ChangeRot(float x, float y, float z)
    {
        Vector3 rot = new Vector3(x, y, z);
        Quaternion q;
        q = Quaternion.Euler(rot);
        transform.rotation = q;
    }
}
