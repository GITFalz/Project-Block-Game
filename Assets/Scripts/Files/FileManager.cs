using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public string worldPacksFolderPath;
    public string executeOnEnterPath;

    public void Init()
    {
        worldPacksFolderPath = Path.Combine(Application.persistentDataPath, "WorldPacks");
        
        if (!Directory.Exists(worldPacksFolderPath))
        {
            Directory.CreateDirectory(worldPacksFolderPath);
        }
        
        executeOnEnterPath = Path.Combine(Application.persistentDataPath, "LoadOnEnter");
        
        if (!Directory.Exists(executeOnEnterPath))
        {
            Directory.CreateDirectory(executeOnEnterPath);
        }
    }
    
    public static Dictionary<string, string> GetFolderNames(string path)
    {
        Dictionary<string, string> names = new Dictionary<string, string>();
        
        if (Directory.Exists(path))
        {
            DirectoryInfo[] subFolders = new DirectoryInfo(path).GetDirectories();

            foreach (DirectoryInfo folder in subFolders)
            {
                names.Add(folder.Name, folder.FullName);
            }
        }
        else
        {
            PopupError.Popup("Directory does not exist");
        }

        return names;
    }
    
    public static List<string> GetFolderPaths(string path)
    {
        List<string> paths = new List<string>();
        
        if (Directory.Exists(path))
        {
            DirectoryInfo[] subFolders = new DirectoryInfo(path).GetDirectories();

            foreach (DirectoryInfo folder in subFolders)
            {
                paths.Add(folder.FullName);
            }
        }
        else
        {
            PopupError.Popup("Directory does not exist");
        }

        return paths;
    }
}