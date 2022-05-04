using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene_Volume : Action_Volume
{
    public PlayableDirector cutscene;

    new private void OnEnable()
    {
        base.OnEnable();
        action += Start_Cutscene;
    }

    void Start_Cutscene(GameObject actor)
    {
        cutscene.Play();
    }
}
