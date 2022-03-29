using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detection : MonoBehaviour
{
    public LayerMask detectable;
    public List<Detection_Cast> casts;
    public Collider collider;

    Vector3 bottom, top;

    public float threshold;

    private void Update()
    {
        bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);
        top = collider.transform.position + new Vector3(0, collider.bounds.extents.y, 0);

        foreach (Detection_Cast c in casts){
            c.Cast(collider.transform.position);
        }

        //Debug.Log(Get_Wall_Angle(threshold));
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

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(down.point, 0.1f);
            Gizmos.DrawSphere(right.point, 0.1f);
            Gizmos.DrawSphere(left.point, 0.1f);

            Gizmos.DrawLine(bottom, transform.position + new Vector3(threshold, 0, 0));
            Gizmos.DrawLine(top, transform.position + new Vector3(threshold, 0, 0));
            Gizmos.DrawLine(bottom, transform.position - new Vector3(threshold, 0, 0));
            Gizmos.DrawLine(top, transform.position - new Vector3(threshold, 0, 0));

            Gizmos.DrawSphere(L1.point, 0.1f);
            Gizmos.DrawSphere(L2.point, 0.1f);
            Gizmos.DrawSphere(R1.point, 0.1f);
            Gizmos.DrawSphere(R2.point, 0.1f);
        }
    }

    RaycastHit down, right, left;
    /// <summary>Returns the incline of the current slope in degrees.</summary>
    public float Get_Slope_Angle()
    {
        Physics.Raycast(bottom, Vector3.right, out right, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, Vector3.left, out left, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, -Vector3.up, out down, collider.bounds.extents.y, detectable);

        float a = right.collider ? right.distance : left.distance;
        float b = Vector2.Distance(right.point, down.point);
        float x;

        float f = a / b;
        x = Mathf.Rad2Deg * Mathf.Asin(f);
        x = float.IsNaN(x) ? 0 : x;
        return x;
    }


    RaycastHit R1, R2, L1, L2;
    public float Get_Wall_Angle(float threshold)
    {
        Vector3 offset = (top - bottom) / 2;
        float max = Vector2.Distance(top, transform.position + new Vector3(threshold, 0, 0));
        Physics.Raycast(bottom, new Vector3(threshold, 0, 0) + offset, out R1, max, detectable);
        Physics.Raycast(top, new Vector3(threshold, 0, 0) - offset, out R2, max, detectable);
        Physics.Raycast(bottom, -new Vector3(threshold, 0, 0) + offset, out L1, max, detectable);
        Physics.Raycast(top, -new Vector3(threshold, 0, 0) - offset , out L2, max, detectable);

        if(L1.collider && L2.collider)
        {
            float a = Vector2.Distance(L1.point, L2.point);
            float b = Mathf.Abs(L1.point.x - L2.point.x);
            float f = b / a;
            float x = Mathf.Rad2Deg * Mathf.Asin(f);
            x = float.IsNaN(x) ? 0 : x;
            return x;

        }
        else if(R1.collider && R2.collider)
        {
            float a = Vector2.Distance(R1.point, R2.point);
            float b = Mathf.Abs(R1.point.x - R2.point.x);
            float f = b / a;
            float x = Mathf.Rad2Deg * Mathf.Asin(f);
            x = float.IsNaN(x) ? 0 : x;
            return x;
        }

        return -1;
    }

    Vector2 dir;
    /// <summary>Returns the direction of the current slope as a Vector2.</summary>
    public Vector2 Get_Slope_Direction()
    {
        Vector3 bottom = collider.transform.position - new Vector3(0, collider.bounds.extents.y, 0);
        Physics.Raycast(bottom, Vector3.right, out right, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, Vector3.left, out left, collider.bounds.extents.x + 0.1f, detectable);
        Physics.Raycast(bottom, -Vector3.up, out down, collider.bounds.extents.y, detectable);

        if (right.collider && down.collider)
        {
            Debug.Log("Right");
            return (right.point - down.point).normalized;
        }
        else if(left.collider && down.collider)
        {
            Debug.Log("Left");
            return (down.point - left.point).normalized;
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
