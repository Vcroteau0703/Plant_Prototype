using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHazard : Hazard
{
    public AudioClip[] dripSounds;
    public ParticleSystem part;
    AudioSource aS;

    bool isPlayed;
    int r;

    private void Awake()
    {
        part = GetComponent<ParticleSystem>();
        aS = GetComponentInChildren<AudioSource>();
    }

    private IEnumerator OnParticleCollision(GameObject other)
    {
        if (!isPlayed)
        {
            DripSFX();
            isPlayed = true;
            if (other.gameObject.TryGetComponent(out IDamagable a))
            {
                a.Damage(damage);
            }
            yield return new WaitForSeconds(0.2f);
            isPlayed = false;
        }


    }

    public void DripSFX()
    {
        r = Random.Range(0, dripSounds.Length);
        aS.clip = dripSounds[r];
        aS.Play();
    }
}
