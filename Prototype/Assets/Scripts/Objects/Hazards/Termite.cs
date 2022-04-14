using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termite : Hazard
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<Player>(out Player p))
        {
            if (p.sapActive)
            {
                gameObject.GetComponentInParent<Termite_Tracker>().Update_Termite_Count();
                gameObject.SetActive(false);
            }
            else if (other.gameObject.TryGetComponent(out IDamagable a))
            {
                a.Damage(damage);
            }
        }
    }
}
