using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public LayerMask detectable;
    public List<Detection_Cast> casts = new List<Detection_Cast>();
    public Collider collider;

    private void Update()
    {
        foreach(Detection_Cast c in casts){
            c.Cast();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,0,1, 0.4f);
        foreach (Detection_Cast c in casts)
        {
            Gizmos.DrawCube(c.center, c.halfExtends * 2);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(down.point, 0.1f);
        Gizmos.DrawSphere(right.point, 0.1f);

        Gizmos.DrawLine(collider.transform.position, dir * 2);

    }

    RaycastHit down, right;
    /// <summary>Returns the incline of the current slope in degrees.</summary>
    public float Get_Slope_Angle()
    {
        //Calculates slope angle from the bottom of the collider forming a right angle

        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);      

        Ray ray_01 = new Ray(bottom, collider.transform.right);
        Ray ray_02 = new Ray(bottom, -collider.transform.up);

        Physics.Raycast(ray_01, out right, detectable);
        Physics.Raycast(ray_02, out down, detectable);

        float a = right.distance, b = Vector2.Distance(right.point, down.point), x;

        float f = a / b;
        x = Mathf.Rad2Deg * Mathf.Asin(f);
        x = float.IsNaN(x) ? 0 : x;
        Debug.Log("a = " + a + " b = " + b + " x = " + x);
        return x;
    }

    Vector2 dir;
    /// <summary>Returns the direction of the current slope as a Vector2.</summary>
    public Vector2 Get_Slope_Direction()
    {
        float angle = Get_Slope_Angle();
        dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        return dir;
    }


}

[System.Serializable]
public struct Detection_Cast
{
    public GameObject target;
    public Vector3 center, halfExtends, direction;
    public float distance;
    public LayerMask mask;

    public void Cast()
    {
        RaycastHit[] hit = Physics.BoxCastAll(center, halfExtends, direction, Quaternion.identity, distance, mask);
        target = hit[0].collider.gameObject;
    }

}
