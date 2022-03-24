using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public LayerMask detectable;
    public List<Detection_Cast> casts;
    public Collider collider;

    private void Update()
    {
        foreach(Detection_Cast c in casts){
            c.Cast(collider.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,0,1, 0.4f);
        foreach (Detection_Cast c in casts)
        {
            Gizmos.color = new Color(0, 0, 1, 0.4f);
            if (c.target != null)
            {
                Gizmos.color = new Color(0, 1, 1, 0.4f);
            }
            Gizmos.DrawCube(collider.transform.position + c.center, c.halfExtends * 2);
        }

        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(down.point, 0.1f);
        Gizmos.DrawSphere(right.point, 0.1f);
    }

    RaycastHit down, right;
    /// <summary>Returns the incline of the current slope in degrees.</summary>
    public float Get_Slope_Angle()
    {
        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);      
        Physics.Raycast(bottom, Vector3.right, out right, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, -Vector3.up, out down, collider.bounds.extents.y, detectable);

        float a = right.distance, b = Vector2.Distance(right.point, down.point), x;

        float f = a / b;
        x = Mathf.Rad2Deg * Mathf.Asin(f);
        x = float.IsNaN(x) ? 0 : x;
        return x;
    }

    Vector2 dir;
    /// <summary>Returns the direction of the current slope as a Vector2.</summary>
    public Vector2 Get_Slope_Direction()
    {
        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);
        Physics.Raycast(bottom, Vector3.right, out right, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, -Vector3.up, out down, collider.bounds.extents.y, detectable);

        if (right.collider && down.collider)
        {
            return (right.point - down.point).normalized;
        }
        return Vector3.right;
    }


}

[System.Serializable]
public class Detection_Cast
{
    public string name;
    public GameObject target;
    public Vector3 center, halfExtends;
    public LayerMask mask;

    public void Cast(Vector3 position)
    {
        Collider[] hit = Physics.OverlapBox(position + center, halfExtends, Quaternion.identity, mask);
        target = hit.Length >= 1 ? hit[0].gameObject : null;
        if(target != null)
        {
            Debug.Log("Found: " + target);
        }
    }

}
