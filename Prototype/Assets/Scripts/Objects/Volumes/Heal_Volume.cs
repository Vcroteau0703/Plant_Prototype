using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Volume : Action_Volume
{
    public Collider col;

    private void Awake()
    {
        col = TryGetComponent(out Collider c) ? c : null;
    }

    private void OnEnable()
    {
        action = Heal_Player;
    }

    public void Heal_Player(GameObject actor)
    {
        Debug.Log("Got Here 1");
        Player player = actor.GetComponent<Player>();
        player.Heal(1);
    }
}
