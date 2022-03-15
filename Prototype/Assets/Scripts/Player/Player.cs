using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public Player_Control_Settings sapSettings;

    private Player_Control_Settings originalSettings;

    public int health = 1;
    public int max_health = 1;
    public Color[] healthColors;

    private SpriteRenderer sprigSprite;
    private Color sprigColor;

    internal bool sapActive = false;
    private bool damageActive = true;

    private void Awake()
    {
        Portal_Data data = SaveSystem.Load<Portal_Data>("/Temp/Portal.data");
        foreach (Portal_Volume v in FindObjectsOfType<Portal_Volume>())
        {
            if (data == null) { break; }
            else if (v.Portal_ID == data.destination) { 
                transform.position = v.transform.position + (Vector3)v.Spawn_Offset;
            }
        }
        sprigSprite = GetComponent<SpriteRenderer>();
        sprigColor = sprigSprite.color;
        originalSettings = gameObject.GetComponent<Player_Controller>().settings;
    }

    public void Damage(int amount)
    {
        if (damageActive && amount != 0)
        {
            health -= amount;
            if (health < 0) { health = 0; Respawn(); }
            else if (health == 0) { Respawn(); }
            else { damageActive = false; }
            ChangeSprigColor(health);
        }
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
                    SapEffectOff();
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    transform.position = x.transform.position;
                    return;
                }
            }
        }
    }

    public void SapEffectOn()
    {
        // make termites collectable and disable their hazard damage
        sapActive = true;
        transform.GetChild(2).gameObject.SetActive(true);
        // set slow movement settings
        gameObject.GetComponent<Player_Controller>().settings = sapSettings;
    }

    public void SapEffectOff()
    {
        sapActive = false;
        transform.GetChild(2).gameObject.SetActive(false);
        gameObject.GetComponent<Player_Controller>().settings = originalSettings;
    }

    public void ChangeSprigColor(int currHealth)
    {
        if(currHealth >= 0)
        {
            sprigSprite.color = sprigColor = healthColors[currHealth - 1];
            if(currHealth != max_health)
            {
                StartCoroutine(IFrames(10, 0.2f));
            }
        }
    }

    public void Heal()
    {
        if(health < max_health)
        {
            health = max_health;
            ChangeSprigColor(health);
        }
    }

    public IEnumerator IFrames(int flashCycles, float cycleTime)
    { 
        float a = 1f, b = 0.5f;
        while (flashCycles > 0)
        {
            float currentTime = 0;
            while (currentTime < cycleTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / cycleTime;
                float currAlpha = Mathf.Lerp(a, b, t);
                sprigColor.a = currAlpha;
                sprigSprite.color = sprigColor;
                yield return null;
            }
            // swapping a and b
            a = a + b;
            b = a - b;
            a = a - b;
            flashCycles--;
        }
        damageActive = true;
    }
}
