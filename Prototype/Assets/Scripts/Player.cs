using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public int health = 1;
    public int max_health = 1;

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0) { health = 0; Respawn();}
        else if(health == 0) { Respawn(); } 
    }

    public void Respawn()
    {
        foreach (MonoBehaviour m in FindObjectsOfType<MonoBehaviour>())
        {
            Checkpoint x = null;
            if (m.TryGetComponent(out ICheckpoint c))
            {
                x = c.Get_Active_Checkpoint();
                if(x != null) {
                    health = max_health;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    transform.position = x.transform.position; 
                    return; 
                }
            }
        }
    }
}
