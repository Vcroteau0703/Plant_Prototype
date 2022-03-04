using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class DialogCutsceneController : MonoBehaviour
{
    private PlayableDirector cutscene;

    private void Awake()
    {
        cutscene = transform.GetComponent<PlayableDirector>();
    }

    [YarnCommand("Camera_Pan")]
    public void StartCutscene()
    {
        cutscene.Stop();       
        cutscene.Play();
    }
}
