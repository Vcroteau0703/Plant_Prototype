using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Boulder : Hazard
{
    public PlayableDirector director;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable a))
        {
            a.Damage(damage);
            ResetCutscene();
        }
    }

    public void ResetCutscene()
    {
        director.Stop();
        director.time = 0;
    }
}
