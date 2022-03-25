using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        float modifier = transform.parent.localScale.x <= 0 ? -1 : 1;
        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);      
        Physics.Raycast(bottom, Vector3.right * modifier, out right, collider.bounds.extents.x + 0.1f, detectable);
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
        float modifier = transform.parent.localScale.x <= 0 ? -1 : 1;
        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);
        Physics.Raycast(bottom, Vector3.right * modifier, out right, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, -Vector3.up, out down, collider.bounds.extents.y, detectable);

        if (right.collider && down.collider)
        {
            return (right.point - down.point).normalized * modifier;
        }
        return Vector3.right;
    }

    /// <summary>Returns true if an object is found during a detection scan.</summary>
    public bool Is_Detecting()
    {
        foreach(Detection_Cast c in casts)
        {
            if(c.target != null)
            {
                return true;
            }
        }
        return false;
    }

    public Detection_Cast Get_Detection(string name)
    {
        foreach(Detection_Cast c in casts)
        {
            if(c.name == name)
            {
                return c;
            }
        }
        return null;
    }

    public List<Detection_Cast> Get_All_Detections()
    {
        List<Detection_Cast> result = new List<Detection_Cast>();

        foreach (Detection_Cast c in casts)
        {
            if (c.target != null)
            {
                result.Add(c);
            }
        }

        return result;
    }
}

[System.Serializable]
public class Detection_Cast
{
    public string name;
    public GameObject target;
    public Vector3 center, halfExtends;
    public LayerMask mask;
    public UnityEvent OnDetectionEnter;
    public UnityEvent OnDetectionExit;

    public void Cast(Vector3 position)
    {
        Collider[] hit = Physics.OverlapBox(position + center, halfExtends, Quaternion.identity, mask, QueryTriggerInteraction.Ignore);
        GameObject found = hit.Length >= 1 ? hit[0].gameObject : null;
        
        if(found != target)
        {
            target = found;
            if(target == null)
            {
                OnDetectionExit.Invoke();
            }
            else
            {
                OnDetectionEnter.Invoke();
            }
            
        }
    }

}
