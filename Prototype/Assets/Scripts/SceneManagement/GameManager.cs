using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static void SaveGame(bool show_notify)
    {
        GameObject[] sceneObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            if (sceneObjects[i].TryGetComponent(out ISavable obj))
            {
                obj.Save();
            }
        }
        if (show_notify)
        {
            Notification_System.Send_SystemNotify("The game has been saved");
        }
    }
    public static void SaveGame()
    {
        GameObject[] sceneObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            if (sceneObjects[i].TryGetComponent(out ISavable obj))
            {
                obj.Save();
            }
        }
        Notification_System.Send_SystemNotify("The game has been saved");       
    }
}
