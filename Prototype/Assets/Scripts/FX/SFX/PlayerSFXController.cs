using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    Player_Controller pC;
    public Detection detection;
    bool left, right, down;
    int r;
    Detection_Cast cast;
    public AudioClip[] footstepsWood;
    public AudioClip[] footstepsEarth;
    public AudioClip[] footstepsStone;
    public AudioClip[] footstepsWater;
    public AudioClip[] damage;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip glide;
    AudioSource aS;
    private Player player;

    public ParticleSystem dustParticle;
    public ParticleSystem dustLandingParticle;
    public ParticleSystem dustWallLandingParticle;
    public ParticleSystem splashParticle;
    int cnt;
    bool active = false;
       
    // Start is called before the first frame update
    private void Awake()
    {
        aS = GetComponent<AudioSource>();
        player = gameObject.GetComponent<Player>();
        pC = gameObject.GetComponent<Player_Controller>();
    }

    void DetermineMaterial()
    {
        if (player.inWater)
        {
            r = Random.Range(0, footstepsWater.Length);
            aS.clip = footstepsWater[r];
        }
        else
        {
            down = detection.Get_Detection("Down").target;
            left = detection.Get_Detection("Left").target;
            right = detection.Get_Detection("Right").target;

            if (down) { cast = detection.Get_Detection("Down"); }
            else if (left) { cast = detection.Get_Detection("Left"); }
            else if (right) { cast = detection.Get_Detection("Right"); }
            if (cast != null && cast.target != null)
            {
                switch (cast.target.tag)
                {
                    case "Earth":
                        r = Random.Range(0, footstepsEarth.Length);
                        aS.clip = footstepsEarth[r];
                        break;
                    case "Wood":
                        r = Random.Range(0, footstepsWood.Length);
                        aS.clip = footstepsWood[r];
                        break;
                    case "Stone":
                        r = Random.Range(0, footstepsStone.Length);
                        aS.clip = footstepsStone[r];
                        break;
                    default:
                        Debug.Log("object not tagged");
                        break;
                }
            }
        }
    }


    public void PlayFootsteps()
    {
        if(cnt < 1)
        {
            DetermineMaterial();
            aS.Play();
            if (!player.inWater)
            {
                if (transform.localScale.x < 0 && dustParticle.transform.localScale.x > 0)
                {
                    dustParticle.transform.localScale = new Vector3(-dustParticle.transform.localScale.x, dustParticle.transform.localScale.y, dustParticle.transform.localScale.z);
                }
                else if (transform.localScale.x > 0 && dustParticle.transform.localScale.x < 0)
                {
                    dustParticle.transform.localScale = new Vector3(-dustParticle.transform.localScale.x, dustParticle.transform.localScale.y, dustParticle.transform.localScale.z);
                }
                dustParticle.Emit(1);
            }
        }
        else
        {
            cnt = 0;
        }

    }

    public void PlayDamage()
    {
        r = Random.Range(0, damage.Length);
        aS.clip = damage[r];
        aS.Play();
    }

    public void PlayDeath()
    {
        aS.clip = death;
        aS.Play();
    }

    public void PlayJump()
    {
        aS.clip = jump;
        aS.Play();
        if (player.inWater)
        {
            splashParticle.Emit(1);
        }
        else
        {
            dustLandingParticle.Emit(1);
        }
    }
    
    public void PlayLanding()
    {
        DetermineMaterial();
        aS.Play();
        cnt = 1;
        if (player.inWater)
        {
            splashParticle.Emit(1);
        }
        else
        {
            dustLandingParticle.Emit(1);
        }
    }

    public void PlayWallLanding()
    {
        DetermineMaterial();
        aS.Play();
        if (transform.localScale.x < 0 && dustWallLandingParticle.transform.localScale.x > 0)
        {
            dustWallLandingParticle.transform.localScale = new Vector3(-dustWallLandingParticle.transform.localScale.x, dustWallLandingParticle.transform.localScale.y, dustWallLandingParticle.transform.localScale.z);
        }
        else if (transform.localScale.x > 0 && dustWallLandingParticle.transform.localScale.x < 0)
        {
            dustWallLandingParticle.transform.localScale = new Vector3(-dustWallLandingParticle.transform.localScale.x, dustWallLandingParticle.transform.localScale.y, dustWallLandingParticle.transform.localScale.z);
        }
        dustWallLandingParticle.Emit(1);
    }

    public void WallSlide()
    {
        if (!active)
        {
            aS.clip = slide;
            aS.loop = true;
            aS.Play();
            active = true;
        }
    }
    
    public void Glide()
    {
        if (!active)
        {
            aS.clip = glide;
            aS.Play();
            active = true;
        }
    }

    public void DeactivateSFX()
    {
        active = false;
        aS.loop = false;
        aS.Stop();
    }

    public void EnablePlayer()
    {
        pC.controls.Player.Move.Enable();
        pC.controls.Player.Jump.Enable();
        //pC.controls.Player.Enable();
    }

    public void DisablePlayer()
    {
        pC.controls.Player.Move.Disable();
        pC.controls.Player.Jump.Disable();
    }

}
