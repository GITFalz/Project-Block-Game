using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static string WorldPacksFolderPath;
    public static string ExecuteOnEnterFolderPath;
    public static string ChunkDataFolderPath;
    public static string WorldFolderPath;
    public static string EditorFolderPath;
    public static string ConfigFolderPath;
    public static string ProjectPath;

    public void Init()
    {
        ProjectPath = Application.persistentDataPath;
        WorldPacksFolderPath = GenerateFolder("WorldPacks");
        ExecuteOnEnterFolderPath = GenerateFolder("LoadOnEnter");
        ChunkDataFolderPath = GenerateFolder("ChunkData");
        WorldFolderPath = GenerateFolder("Worlds");
        ConfigFolderPath = GenerateFolder("Config");
        EditorFolderPath = GenerateFolder("Editor");
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
    
    public static bool Save(string path, string text)
    {
        if (path.Equals(""))
            return false;
        
        File.WriteAllText(path, text);
        return true;
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

    private static string GenerateFolder(string folderName)
    {
        string path = Path.Combine(ProjectPath, folderName);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }
}