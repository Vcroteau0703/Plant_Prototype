using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termite : Hazard
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable a))
        {
            a.Damage(damage);
        }
    }
}
