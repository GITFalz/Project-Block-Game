using System;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CWorldEditorManager : MonoBehaviour
{
    public static CWorldEditorManager Instance;
    
    public TMP_InputField inputField;
    public TextureGeneration textureGeneration;
    public CWorldMenu menu;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private bool CanExecute()
    {
        return gameObject != null && gameObject.scene.isLoaded;
    }
    
    public async void ExecuteCode()
    {
        ChunkGenerationNodes.Clear();
        ChunkGenerationNodes.Set();
        await CWorldCommandManager.Load(menu.currentFilePath);
    }

    public async void SaveFile()
    {
        await CWorldCommandManager._writerManager.InitLines(inputField.text);
        await SaveFileAsync();
    }
    
    public Task<bool> SaveFileAsync()
    {
        if (!CanExecute()) return Task.FromResult(false);
        
        Console.Log("Saving file...");

        bool quitNext = false;
        foreach (string value in CWorldCommandManager.Args)
        {
            if (quitNext)
            {
                string path = value + ".cworld";
                FileManager.Save(menu.currentFolderPath, path, inputField.text);
                break;
            }

            if (value.Equals("Save"))
            {
                quitNext = true;
            }
        }
        Console.Log("File saved!");
        
        return Task.FromResult(true);
    }

    public void UpdateTexture()
    {
        Debug.Log("Drawing texture...");
        
        textureGeneration.SetMove(true);

        if (ChunkGenerationNodes.sampleDisplayName.Equals(""))
            Console.Log("No sample display name");
            
        Console.Log(">> Drawing texture...");
        textureGeneration.UpdateTexture(ChunkGenerationNodes.sampleDisplayName);
    }
    
    public void DisplayContent(string filePath)
    {
        try
        {
            string content = File.ReadAllText(filePath);
            inputField.text = content;
        }
        catch (FileNotFoundException)
        {
            Console.Log("File not found");
        }
    }
    
    public void Clear()
    {
        inputField.text = "";
    }
}