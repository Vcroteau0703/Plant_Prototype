using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class Gold_Leaf : Action_Volume
{
    public GameObject pickupParticle;

    private void Awake()
    {
        Player_Data data = SaveSystem.Load<Player_Data>("/Player/Player.data");
        if (data == null) { return; }

        int localID = ((int)transform.position.sqrMagnitude);

        foreach (Gold_Leaf_Data saved in data.goldLeaves)
        {
            if (localID == saved.id)
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

        Gold_Leaf_Data[] temp = Player.goldenLeaves;
        
        if(temp == null || temp.Length == 0) {
            Player.goldenLeaves = new Gold_Leaf_Data[1];
            Player.goldenLeaves[0] = new Gold_Leaf_Data(this);
            Destroy(gameObject);
            return;
        }

        Player.goldenLeaves = new Gold_Leaf_Data[temp.Length + 1];

        for (int i = 0; i < temp.Length; i++)
        {
            Player.goldenLeaves[i] = temp[i];
            if(i == temp.Length - 1){
                Player.goldenLeaves[i + 1] = new Gold_Leaf_Data(this);
            }
        }
        Destroy(gameObject);
    }

}

[System.Serializable]
public class Gold_Leaf_Data
{
    [SerializeField] public int id;
    public Gold_Leaf_Data(Gold_Leaf leaf)
    {
        id = ((int)leaf.transform.position.sqrMagnitude);
    }
}
