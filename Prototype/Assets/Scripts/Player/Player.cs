using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamagable, ISavable
{
    public Player_Control_Settings sapSettings;
    private Player_Control_Settings originalSettings;

    public static Gold_Leaf_Data[] goldenLeaves;

    public int health = 1;
    public int max_health = 1;
    public int invCycles;

    public SpriteRenderer[] sprigSprites;
    private Color sprigColor;
    public Color sapColor;
    public Color SprigHurtColor;

    internal bool sapActive = false;
    private bool damageActive = true;
    public bool inWater = false;

    internal GameObject HUD;
    internal int currColl;
    Animator anim;
    Player_Controller pC;

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
            goldenLeaves = player.goldLeaves;
            health = player.health;
            currColl = player.currColl;
            if (SceneManager.GetActiveScene().name == player.scene)
            {
                float[] checkpoint = player.checkpoint.position;
                transform.position = new Vector3(checkpoint[0], checkpoint[1], checkpoint[2]);
            }
        }
        anim = GetComponent<Animator>();
        pC = GetComponent<Player_Controller>();
        originalSettings = gameObject.GetComponent<Player_Controller>().settings;
        HUD = GameObject.FindGameObjectWithTag("HUD");
        HUD.GetComponentInChildren<HealthUI>().UpdateUI(health);
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
            anim.SetLayerWeight(1, 0.5f);
            anim.SetTrigger("HIT");
            if (health == 0)
            {
                anim.SetTrigger("DEATH");
                pC.OnDisable();
                damageActive = false;
            }
            else if (health < 0)
            {
                health = 0;
                pC.OnDisable();
                anim.SetTrigger("DEATH");
                damageActive = false;
            }
            else { damageActive = false; }
            UpdateHealthUI(health);
        }
    }

    public void ResetLayerWeight()
    {
        anim.SetLayerWeight(1, 0f);
    }

    public void Respawn()
    {
        Checkpoint x = Checkpoint.Get_Active_Checkpoint();

        if (x != null)
        {
            health = max_health;
            damageActive = true;
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
        sapActive = true;
        foreach (SpriteRenderer sR in sprigSprites)
        {
            sR.color = sapColor;
        }
        GameObject.Find("SapEffect").GetComponent<ParticleSystem>().Play();
        gameObject.GetComponent<Player_Controller>().settings = sapSettings;
    }

    public void SapEffectOff()
    {
        sapActive = false;
        foreach (SpriteRenderer sR in sprigSprites)
        {
            sR.color = Color.white;
        }
        GameObject.Find("SapEffect").GetComponent<ParticleSystem>().Stop();
        gameObject.GetComponent<Player_Controller>().settings = originalSettings;
    }

    public void UpdateHealthUI(int currHealth)
    {
        if(currHealth >= 0)
        {
            HUD.GetComponentInChildren<HealthUI>().UpdateUI(currHealth);
            if(currHealth != max_health && currHealth != 0)
            {
                if (sapActive)
                {
                    sprigColor = sapColor;
                }
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
                Color currColor = Color.Lerp(sprigColor, SprigHurtColor, t);
                sprigColor = currColor;
                foreach (SpriteRenderer sR in sprigSprites)
                {
                    sR.color = sprigColor;
                }
                yield return null;
            }
            currentTime = 0;
            while (currentTime < cycleTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / cycleTime;
                if (sapActive)
                {
                    Color currColor = Color.Lerp(sprigColor, sapColor, t);
                    sprigColor = currColor;
                }
                else
                {
                    Color currColor = Color.Lerp(sprigColor, Color.white, t);
                    sprigColor = currColor;
                }
                foreach (SpriteRenderer sR in sprigSprites)
                {
                    sR.color = sprigColor;
                }
                yield return null;
            }
            flashCycles--;
        }
        damageActive = true;
    }
}

[System.Serializable]
public class Player_Data
{
     //THIS CLASS CONTAINS THE DATA SAVED IN PATH: SaveFile/Player/Player.data

    public int health;
    public int currColl;
    public string currentQuest;
    public string scene;
    public Checkpoint_Data checkpoint;
    public Gold_Leaf_Data[] goldLeaves;

    public Player_Data(Player player)
    {
        //STATS
        health = player.health;
        currColl = player.currColl;

        //COLLECTABLES
        goldLeaves = Player.goldenLeaves;

        //SCENES
        scene = SceneManager.GetActiveScene().name;

        //QUEST SYSTEM
        Quest current = Quest_Handler.instance.Target_Quest;
        currentQuest = current != null ? Quest_Handler.instance.Target_Quest.name : "NONE";

        //CHECKPOINTS
        Checkpoint c = Checkpoint.Get_Active_Checkpoint();
        checkpoint = c != null ? new Checkpoint_Data(c) : null;
    }
}
