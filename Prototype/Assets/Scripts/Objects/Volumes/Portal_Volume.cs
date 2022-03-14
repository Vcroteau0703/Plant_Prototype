using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal_Volume : Action_Volume
{
    public int Scene_ID;
    public string Portal_ID;
    public string Linked_ID;

    public Vector2 Size = Vector2.one;

    public Vector2 Spawn_Offset;

    private Portal_Data data = new Portal_Data();
    public Portal_Data Data { get { return data; } }

    private BoxCollider col;

    private void OnEnable()
    {
        action = Travel;
    }

    private void Travel(GameObject actor)
    {
        Portal_Data temp = SaveSystem.Load<Portal_Data>("/Temp/Portal.data");
        if(temp != null && temp.destination != "") {
            temp.destination = "";
            data = temp;
            SaveSystem.Save(data, "/Temp/Portal.data");
        } 
        data.destination = Linked_ID;
        SaveSystem.Save(data, "/Temp/Portal.data");
        GameManager.SaveGame();
        SceneManager.LoadScene(Scene_ID);
    }

    private void OnDrawGizmos()
    {
        if(col == null && TryGetComponent(out BoxCollider c)){col = c;}
        col.size = Size;

        Gizmos.color = Color.blue * new Color(1,1,1,0.5f);
        Gizmos.DrawCube(transform.position, Size);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, Size);
        Gizmos.DrawIcon(transform.position, "Import@2x", true, Color.white);

        Gizmos.color = Color.blue * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + (Vector3)Spawn_Offset, new Vector3(1, 2.2f, 1));
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position + (Vector3)Spawn_Offset, new Vector3(1, 2.2f, 1));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)Spawn_Offset);
        Gizmos.DrawIcon(transform.position + (Vector3)Spawn_Offset, "AvatarSelector@2x", true, new Color(0f,1f,0.2f));
    }
}

[System.Serializable]
public class Portal_Data
{
    public string destination;
}
