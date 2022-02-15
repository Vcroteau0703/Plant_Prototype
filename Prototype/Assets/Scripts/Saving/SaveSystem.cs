using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    public static string CurrentSave;

    public static string[] GetSaveNames()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] info = dir.GetDirectories();
        string[] fileNames = new string[info.Length];

        for(int i = 0; i < info.Length; i++)
        {
            fileNames[i] = info[i].Name;
        }
        return fileNames;
    }
    public static void CreateNewSave(string name)
    {
        string p_Main = Path.Combine(Application.persistentDataPath, name);
        var directory = Directory.CreateDirectory(p_Main);
        CurrentSave = directory.FullName;

        string p_Levels = Path.Combine(CurrentSave, "Levels");
        Directory.CreateDirectory(p_Levels);

        

        string p_Player = Path.Combine(CurrentSave, "Player");
        Directory.CreateDirectory(p_Player);
    }

    public static void DeleteSave(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, name);
        string[] dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

        for(int i = dirs.Length-1; i >= 0; i--)
        {
            string[] files = Directory.GetFiles(dirs[i]);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(dirs[i]);
        }
        Directory.Delete(path);
    }

    //#region Player
    //public static void SavePlayerData(PlayerData data)
    //{       
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    string path = Path.Combine(CurrentSave, "Player")  + "/player.data";
    //    FileStream stream = new FileStream(path, FileMode.Create);
    //    formatter.Serialize(stream, data);
    //    stream.Close();
    //}
    //public static PlayerData LoadPlayerData()
    //{
    //    string path = Path.Combine(CurrentSave, "Player") + "/player.data";
    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        PlayerData data = formatter.Deserialize(stream) as PlayerData;
    //        stream.Close();

    //        return data;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
    //#endregion

    //#region NPC
    //public static void SaveNPCData(NPCData data)
    //{
    //    string level = Path.Combine(CurrentSave, "Levels", data.scene);
    //    if (!Directory.Exists(level)){
    //        Directory.CreateDirectory(level);
    //    }
    //    string p_NPC = Path.Combine(level, "NPC");     
    //    if (!Directory.Exists(p_NPC)){
    //        Directory.CreateDirectory(p_NPC);
    //    }

    //    BinaryFormatter formatter = new BinaryFormatter();
    //    string path = p_NPC + "/" + data.name + ".data";
    //    FileStream stream = new FileStream(path, FileMode.Create);
    //    formatter.Serialize(stream, data);
    //    stream.Close();
    //}
    //public static NPCData LoadNPCData(string scene, string name)
    //{
    //    string NPC = Path.Combine(CurrentSave, "Levels", scene, "NPC");
    //    string path = NPC + "/" + name + ".data";
    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        NPCData data = formatter.Deserialize(stream) as NPCData;
    //        stream.Close();

    //        return data;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    //public static NPCData[] LoadAllNPCData(string scene)
    //{
    //    string path = Path.Combine(CurrentSave, "Levels", scene, "NPC");
    //    if (!Directory.Exists(path))
    //    {
    //        return null;
    //    }
    //    DirectoryInfo dir = new DirectoryInfo(path);
    //    FileInfo[] info = dir.GetFiles();

    //    NPCData[] dataFiles = new NPCData[info.Length];

    //    for (int i = 0; i < info.Length; i++)
    //    {
    //        string name = info[i].Name.Replace(".data", null);
    //        dataFiles[i] = LoadNPCData(scene, name);
    //    }
    //    return dataFiles;

    //}
    //#endregion
}
