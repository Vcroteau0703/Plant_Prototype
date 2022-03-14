using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamagable, ISavable
{
    public int health = 1;
    public int max_health = 1;
    public Color[] healthColors;

    private SpriteRenderer sprigSprite;
    private Color sprigColor;

    internal bool sapActive = false;
    private bool damageActive = true;

    private void Awake()
    {
        Portal_Data portal = SaveSystem.Load<Portal_Data>("/Temp/Portal.data");
        foreach (Portal_Volume v in FindObjectsOfType<Portal_Volume>())
        {
            if (portal == null) { break; }
            else if (v.Portal_ID == portal.destination) { 
                transform.position = v.transform.position + (Vector3)v.Spawn_Offset;
            }
        }

        Player_Data player = SaveSystem.Load<Player_Data>("/Player/Player.data");
        if (player != null){
            health = player.health;
            float[] checkpoint = player.checkpoint.position;
            transform.position = new Vector3(checkpoint[0], checkpoint[1], checkpoint[2]);
        }
        sprigSprite = GetComponent<SpriteRenderer>();
        sprigColor = sprigSprite.color;
    }

    public void Save()
    {
        Debug.Log("Player Saved");
        Player_Data data = new Player_Data(this);
        SaveSystem.Save(data, "/Player/Player.data");
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
        Checkpoint x = Checkpoint.Get_Active_Checkpoint();

        if (x != null)
        {
            health = max_health;
            ChangeSprigColor(health);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = x.transform.position;
        }
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

    public void Heal(int amount)
    {
        if(health < max_health)
        {
            Debug.Log("got here");
            health += amount;
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

[System.Serializable]
public class Player_Data
{
    public int health;
    public string scene;
    public Checkpoint_Data checkpoint;

    public Player_Data(Player player)
    {
        health = player.health;
        scene = SceneManager.GetActiveScene().name;

        Checkpoint c = Checkpoint.Get_Active_Checkpoint();
        checkpoint = c != null ? new Checkpoint_Data(c) : null;
    }
}
