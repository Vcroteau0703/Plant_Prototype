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
        if(data == null)
        {
            ActionWindow.ButtonFunction function = Delete;
            Notification_System.Send_ActionWindow("Player.data does not exist in " + fileName.text + ". File might be corrupt or lost, Would you like to delete the save?", "Delete", function);
            return;
        }
        Laucher.LoadScene(data.scene);
    }
    public void Delete()
    {
        SaveSystem.DeleteSave(fileName.text);
        Destroy(gameObject);
    }
}
