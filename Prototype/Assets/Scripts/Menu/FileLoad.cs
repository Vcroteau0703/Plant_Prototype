using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class FileLoad : MonoBehaviour
{
    public TMP_Text fileName;

    public void LoadSave()
    {
        SaveSystem.CurrentSave = Path.Combine(Application.persistentDataPath, fileName.text);
        Player_Data data = SaveSystem.Load<Player_Data>("/Player/Player.data");
        Laucher.LoadScene(data.scene);
    }

    public void DeleteSave()
    {
        ActionWindow.ButtonFunction function = Delete;
        Notification_System.Send_ActionWindow("Do you want to delete " + fileName.text + "?", "Delete", function);
    }

    public void Delete()
    {
        SaveSystem.DeleteSave(fileName.text);
        Destroy(gameObject);
    }
}
