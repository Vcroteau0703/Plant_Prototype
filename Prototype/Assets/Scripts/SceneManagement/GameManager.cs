using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static void SaveGame(bool show_notify)
    {
        MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>(true);
        foreach (MonoBehaviour m in monos)
        {
            foreach(ISavable a in m.GetComponents<ISavable>())
            {
                a.Save();
            }
        }

        if (show_notify)
        {
            Notification_System.Send_SystemNotify("The game has been saved");
        }
    }
    public static void SaveGame()
    {
        MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>(true);
        foreach (MonoBehaviour m in monos)
        {
            foreach (ISavable a in m.GetComponents<ISavable>())
            {
                a.Save();
            }
        }
        Notification_System.Send_SystemNotify("The game has been saved");       
    }
}
