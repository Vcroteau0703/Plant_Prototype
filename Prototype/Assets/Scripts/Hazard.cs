using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage;

    private void Awake()
    {
        //if (!GetComponent<Collider>()){ Debug.LogError("MISSING COMPONENT: " + this.gameObject.name + " is missing a collider."); };
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out IDamagable a))
        {
            a.Damage(damage);
        }
    }
}
