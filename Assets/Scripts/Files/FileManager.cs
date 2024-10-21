using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public string worldPacksFolderPath;

    private void Start()
    {
        worldPacksFolderPath = Path.Combine(Application.persistentDataPath, "WorldPacks");
        
        if (!Directory.Exists(worldPacksFolderPath))
        {
            Directory.CreateDirectory(worldPacksFolderPath);
        }
    }
}