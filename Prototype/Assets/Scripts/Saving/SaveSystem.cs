using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    public static string CurrentSave = "";

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


    /* PATH FORMATING
     * 
     * Path parameter will be added on to the Current Save path:
     * 
     * Current Save: Application.persistentDataPath/"Save Name"
     * 
     * EX. /Player/Player.data
     * EX. /Levels/Level_01.data
     */

    public static void Save<T>(T data, string path)
    {        
        BinaryFormatter formatter = new BinaryFormatter();
        string p = Application.persistentDataPath + CurrentSave + path;
        Debug.Log(p);
        FileStream stream = new FileStream(p, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static T Load<T>(string path)
    {
        string p = Application.persistentDataPath + CurrentSave + path;
        Debug.Log(p);
        if (File.Exists(p))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(p, FileMode.Open);

            T data = (T)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            return default;
        }
    }
}
