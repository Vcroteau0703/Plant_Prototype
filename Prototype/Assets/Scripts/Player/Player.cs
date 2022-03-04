using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public int health = 1;
    public int max_health = 1;
    public Color[] healthColors;
    private SpriteRenderer sprigSprite;
    internal bool sapActive = false;

    private void Awake()
    {
        Portal_Data data = SaveSystem.Load<Portal_Data>("/Portal.data");
        foreach (Portal_Volume v in FindObjectsOfType<Portal_Volume>())
        {
            if (data == null) { break; }
            else if (v.Portal_ID == data.destination) { 
                transform.position = v.transform.position + (Vector3)v.Spawn_Offset;
            }
        }
        sprigSprite = GetComponent<SpriteRenderer>();
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0) { health = 0; Respawn(); }
        else if (health == 0) { Respawn(); }
        ChangeSprigColor(health);
    }

    public void Respawn()
    {
        foreach (MonoBehaviour m in FindObjectsOfType<MonoBehaviour>())
        {
            Checkpoint x = null;
            if (m.TryGetComponent(out ICheckpoint c))
            {
                x = c.Get_Active_Checkpoint();
                if (x != null) {
                    health = max_health;
                    ChangeSprigColor(health);
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    transform.position = x.transform.position;
                    return;
                }
            }
        }
    }

    public void ChangeSprigColor(int currHealth)
    {
        if(currHealth >= 0)
        {
            sprigSprite.color = healthColors[currHealth - 1];
        }
    }

    public void Heal(int amount)
    {
        if(health < max_health)
        {
            health += amount;
            ChangeSprigColor(health);
        }
    }

    public IEnumerator IFrames()
    {
        yield return new WaitForSeconds(3);
    }
}
