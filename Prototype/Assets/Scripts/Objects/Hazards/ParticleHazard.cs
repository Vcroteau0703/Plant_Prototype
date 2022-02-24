using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHazard : Hazard
{
    public ParticleSystem part;

    private void Awake()
    {
        part = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.tag);
        if (other.gameObject.TryGetComponent(out IDamagable a))
        {
            a.Damage(damage);
        }
    }

}
