using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WMWriter : MonoBehaviour
{
    public static WMWriter Instance;

    public TMP_InputField inputField;
    public TextureGeneration textureGeneration;
    public CWorldMenu menu;

    public bool simpleLoad = false;
    public bool ignoreEverything = false;

    [HideInInspector]
    
    public WriterManager writerManager;
    private string currentPath;
    private string currentFileConent;

    private int Index => writerManager.index;
    private string[] Args => writerManager.args;
    public string Arg => Index < Args.Length ? Args[Index] : null;

    private void Start()
    {
        writerManager = new WriterManager();
        ChunkGenerationNodes.Set();
        
        if (ignoreEverything) 
            return;
        
        LoadOnEnter();
    }

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ClearAll()
    {
        ChunkGenerationNodes.Clear();
    }

    

    public async void LoadOnEnter()
    {
        currentPath = FileManager.ExecuteOnEnterFolderPath;
        if (!currentPath.Equals(""))
        {
            Console.Log("Loading files...");
            string[] files = GetCWorldFilesInFolder();
            string f = files.Length <= 1 ? "file was" : "files were";
            Console.Log(files.Length + $" {f} found");

            int i = 0;
            foreach (var filePath in files)
            {
                Console.Log($"File {i}: " + filePath);
                await CWorldCommandManager.Load(filePath);
                i++;
            }
        }
    }

    

    public string[] GetCWorldFilesInFolder()
    {
        List<string> currentPaths = new List<string>();
        List<string> checkedPaths = new List<string>();
        List<string> toBeChecked = new List<string>();
        List<string> files = new List<string>();
        
        string[] filePaths = Directory.GetFiles(FileManager.ExecuteOnEnterFolderPath, "*.cworld");
        files.AddRange(filePaths);
        
        currentPaths = FileManager.GetFolderPaths(FileManager.ExecuteOnEnterFolderPath);

        while (true)
        {
            foreach (var path in currentPaths)
            {
                var newToBeChecked = FileManager.GetFolderPaths(path);
                toBeChecked.AddRange(newToBeChecked);
                checkedPaths.Add(path);
            }
            currentPaths.Clear();
            currentPaths.AddRange(toBeChecked);
            toBeChecked.Clear();

            if (currentPaths.Count == 0)
                break;
        }
        
        foreach (var path in checkedPaths)
        {
            filePaths = Directory.GetFiles(path, "*.cworld");
            files.AddRange(filePaths);
        }

        return files.ToArray();
    }
}