using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EditTimelineSpeed : MonoBehaviour
{

    private PlayableDirector cutscene;

    public float playbackSpeed;



    private void Awake()
    {
        cutscene = GetComponent<PlayableDirector>();
        cutscene.playableGraph.GetRootPlayable(0).SetSpeed<Playable>(playbackSpeed);
    }

}
