using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamagable, ISavable
{
    public Player_Control_Settings sapSettings;
    private Player_Control_Settings originalSettings;

    public int health = 1;
    public int max_health = 1;
    public int invCycles;

    private SpriteRenderer sprigSprite;
    private Color sprigColor;

    internal bool sapActive = false;
    private bool damageActive = true;

    internal GameObject HUD;
    internal int currColl;

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
            if (SceneManager.GetActiveScene().name == player.scene)
            {
                float[] checkpoint = player.checkpoint.position;
                transform.position = new Vector3(checkpoint[0], checkpoint[1], checkpoint[2]);
            }
        }
        sprigSprite = GetComponent<SpriteRenderer>();
        sprigColor = sprigSprite.color;
        originalSettings = gameObject.GetComponent<Player_Controller>().settings;
        HUD = GameObject.FindGameObjectWithTag("HUD");
        UpdateHealthUI(health);
    }

    public void Save()
    {
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
            UpdateHealthUI(health);
        }
    }

    public void Respawn()
    {
        Checkpoint x = Checkpoint.Get_Active_Checkpoint();

        if (x != null)
        {
            health = max_health;
            UpdateHealthUI(health);
            if (sapActive)
            {
                SapEffectOff();
            }
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = x.transform.position;
            return;
        }   
    }

    public void SapEffectOn()
    {
        // make termites collectable and disable their hazard damage
        sapActive = true;
        transform.Find("Sap").gameObject.SetActive(true);
        // set slow movement settings
        gameObject.GetComponent<Player_Controller>().settings = sapSettings;
    }

    public void SapEffectOff()
    {
        sapActive = false;
        transform.Find("Sap").gameObject.SetActive(false);
        gameObject.GetComponent<Player_Controller>().settings = originalSettings;
    }

    public void UpdateHealthUI(int currHealth)
    {
        if(currHealth >= 0)
        {
            HUD.GetComponentInChildren<HealthUI>().UpdateUI(currHealth);
            if(currHealth != max_health)
            {
                StartCoroutine(IFrames(invCycles, 0.2f));
            }
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
    public int currColl;
    public string scene;
    public Checkpoint_Data checkpoint;

    public Player_Data(Player player)
    {
        health = player.health;
        currColl = player.currColl;
        scene = SceneManager.GetActiveScene().name;

        Checkpoint c = Checkpoint.Get_Active_Checkpoint();
        checkpoint = c != null ? new Checkpoint_Data(c) : null;
    }
}
