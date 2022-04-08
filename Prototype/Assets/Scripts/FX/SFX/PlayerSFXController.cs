using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
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
    AudioSource aS;

    public ParticleSystem dustParticle;
    public ParticleSystem dustLandingParticle;
    public ParticleSystem dustWallLandingParticle;
       
    // Start is called before the first frame update
    private void Awake()
    {
        aS = GetComponent<AudioSource>();
        var main = dustWallLandingParticle.main;
    }

    void DetermineMaterial()
    {
        down = detection.Get_Detection("Down").target;
        left = detection.Get_Detection("Left").target;
        right = detection.Get_Detection("Right").target;

        if (down) { cast = detection.Get_Detection("Down"); }
        else if (left) { cast = detection.Get_Detection("Left"); }
        else if (right) { cast = detection.Get_Detection("Right"); }
        if(cast.target != null)
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
                case "Water":
                    r = Random.Range(0, footstepsWater.Length);
                    aS.clip = footstepsWater[r];
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


    public void PlayFootsteps()
    {
        DetermineMaterial();
        aS.Play();
        if (transform.localScale.x < 0 && dustParticle.transform.localScale.x > 0)
        {
            dustParticle.transform.localScale = new Vector3 (-dustParticle.transform.localScale.x, dustParticle.transform.localScale.y, dustParticle.transform.localScale.z);
        }
        else if (transform.localScale.x > 0 && dustParticle.transform.localScale.x < 0)
        {
            dustParticle.transform.localScale = new Vector3(-dustParticle.transform.localScale.x, dustParticle.transform.localScale.y, dustParticle.transform.localScale.z);
        }
        dustParticle.Emit(1);
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
        dustLandingParticle.Emit(1);
    }
    
    public void PlayLanding()
    {
        DetermineMaterial();
        aS.Play();
        dustLandingParticle.Emit(1);
    }

    public void PlayWallLanding()
    {
        DetermineMaterial();
        aS.Play();
        //var main = dustWallLandingParticle.main;
        //if (transform.localScale.x < 0 && main.startRotationZ.constant > 0)
        //{
        //    main.startRotationZ = 90; 
        //}
        //else if (transform.localScale.x > 0 && main.startRotationZ.constant < 0)
        //{
        //    main.startRotationZ = -90;
        //}
        //dustWallLandingParticle.Emit(1);
    }

}
