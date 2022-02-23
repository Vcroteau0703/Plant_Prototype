using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Reset : MonoBehaviour
{
    public GameObject boulder;
    private PlayableDirector director;

    private void Awake()
    {
        director = transform.GetComponent<PlayableDirector>();
    }

    public void ResetCutscene()
    {
        director.time = 0;
        director.Play();
    }
    
}
