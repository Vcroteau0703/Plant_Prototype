using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_Leaf : Action_Volume
{
    public GameObject pickupParticle;

    private void Awake()
    {
        Player_Data data = SaveSystem.Load<Player_Data>("/Player/Player.data");
        if (data == null) { return; }

        foreach (Gold_Leaf_Data g in data.goldLeaves)
        {
            if (GetInstanceID() == g.id)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnEnable()
    {
        action = Collect;
    }

    public void Collect(GameObject actor)
    {
        Instantiate(pickupParticle, transform.position, Quaternion.identity);

        Player player = actor.GetComponentInParent<Player>();

        Gold_Leaf_Data[] temp = player.goldenLeaves;
        
        if(temp.Length == 0) {
            player.goldenLeaves = new Gold_Leaf_Data[1];
            player.goldenLeaves[0] = new Gold_Leaf_Data(this);
            Destroy(gameObject);
            return;
        }

        player.goldenLeaves = new Gold_Leaf_Data[temp.Length + 1];

        for (int i = 0; i < temp.Length; i++)
        {
            player.goldenLeaves[i] = temp[i];
            if(i == temp.Length - 1){
                player.goldenLeaves[i + 1] = new Gold_Leaf_Data(this);
            }
        }
        Destroy(gameObject);
    }

}

[System.Serializable]
public class Gold_Leaf_Data
{
    public int id;
    public Gold_Leaf_Data(Gold_Leaf leaf)
    {
        id = leaf.GetInstanceID();
    }
}
