using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public int health = 1;

    public void Damage(int amount)
    {
        if(health - amount < 0) { health = 0; Respawn(); return; }
        health -= amount;
    }

    public void Respawn()
    {
        foreach (MonoBehaviour m in FindObjectsOfType<MonoBehaviour>())
        {
            Checkpoint x = null;
            if (m.TryGetComponent(out ICheckpoint c))
            {
                x = c.Get_Active_Checkpoint();
                if(x != null) { transform.position = x.transform.position; return; }
            }
        }
    }
}
