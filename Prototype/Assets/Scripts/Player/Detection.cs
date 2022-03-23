using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public List<Detection_Cast> casts = new List<Detection_Cast>();
    public Collider col;

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
