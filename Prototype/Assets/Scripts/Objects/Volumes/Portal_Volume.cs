using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal_Volume : Action_Volume
{
    public int Scene_ID;
    public string Portal_ID;
    public string Linked_ID;

    public Vector2 Spawn_Offset;

    public Texture2D Icon_Texture;

    private Portal_Data data = new Portal_Data();
    public Portal_Data Data { get { return data; } }

    private void OnEnable()
    {
        action = Travel;
    }

    private void Travel(GameObject actor)
    {
        Portal_Data temp = SaveSystem.Load<Portal_Data>("/Portal.data");
        if(temp != null && temp.destination != "") {
            temp.destination = "";
            data = temp;
            SaveSystem.Save(data, "/Portal.data");
            return; 
        } 
        data.destination = Linked_ID;
        SaveSystem.Save(data, "/Portal.data");
        SceneManager.LoadScene(Scene_ID);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue * new Color(1,1,1,0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(2, 2, 2));
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(2, 2, 2));
        Gizmos.DrawIcon(transform.position, "Import@2x", true, Color.white);

        Gizmos.color = Color.blue * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + (Vector3)Spawn_Offset, new Vector3(1, 1, 1));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Spawn_Offset);
        //Gizmos.DrawIcon(transform.position + (Vector3)Spawn_Offset, "Sprig.png", true, Color.yellow);
        Gizmos.DrawGUITexture(new Rect(transform.position.x, transform.position.y, 1000, 1000), Icon_Texture);
    }
}

[System.Serializable]
public class Portal_Data
{
    public string destination;
}
