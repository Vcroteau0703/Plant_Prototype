using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termite : Hazard
{
    public override void Deal_Damage(GameObject actor)
    {
        if (actor.transform.parent.TryGetComponent(out Player p))
        {
            if (p.sapActive)
            {
                gameObject.GetComponentInParent<Termite_Tracker>().Update_Termite_Count();
                gameObject.GetComponentInParent<TermiteSFX>().PlaySFX();
                if (oneShot)
                {
                    Save_And_Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else if (p.TryGetComponent(out IDamagable a))
            {
                a.Damage(damage);
            }
        }
    }
}
