using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : Action_Volume
{
    public int damage;

    new private void OnEnable()
    {
        base.OnEnable();
        action = Deal_Damage;
    }

    public virtual void Deal_Damage(GameObject actor)
    {
        if (actor.transform.parent.TryGetComponent(out IDamagable a))
        {
            a.Damage(damage);
        }
    }

    public void ChangeDamage(int dmg)
    {
        damage = dmg;
    }
}
