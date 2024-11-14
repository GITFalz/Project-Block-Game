using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CWorldMenu : MonoBehaviour
{
    public FileManager fileManager;
    public WMWriter writer;

    public GameObject buttonPrefab;
    public GameObject deleteButtonPrefab;

    public Transform folderContent;
    public Transform fileContent;

    public TMP_InputField inputField;
    
    public string currentFolderPath;
    public string currentFilePath;
    
    private Dictionary<string, string> worldFolders;
    private string[] worldFiles;

    private HashSet<string> folderNames;
    private HashSet<string> fileNames;

    private List<GameObject> folderButtons;
    private List<GameObject> fileButtons;

    private bool _move;

    private string _selectedFilePath;


    public void Init()
    {
        fileManager.Init();
        currentFolderPath = FileManager.EditorFolderPath;

        folderNames = new HashSet<string>();
        fileNames = new HashSet<string>();
        folderButtons = new List<GameObject>();
        fileButtons = new List<GameObject>();

        _move = false;
        
        GenerateCWorldButtons();
    }

    public void GenerateCWorldButtons()
    {
        foreach (var folder in folderButtons)
            Destroy(folder);
        
        foreach (var file in fileButtons)
            Destroy(file);
        
        folderNames.Clear();
        fileNames.Clear();
        folderButtons.Clear();
        fileButtons.Clear();
        
        GenerateFolderButtons();
        GenerateFileButtons();
    }

    public void GenerateFolderButtons()
    {
        DirectoryInfo dirInfo = Directory.GetParent(currentFolderPath);
        if (dirInfo == null)
        {
            PopupError.Popup("No parent directory found");
            return;
        }
        
        worldFolders = FileManager.GetFolderNames(currentFolderPath);
        
        GenerateFolderButton(new KeyValuePair<string, string>("Back", dirInfo.FullName));
        
        foreach (var file in worldFolders)
        {
            GenerateFolderButton(file);
        }
    }
    
    public void GenerateFileButtons()
    {
        worldFiles = Directory.GetFiles(currentFolderPath, "*.cworld");
        
        foreach (string filePath in worldFiles)
        {
            GenerateFileButton(filePath);
        }
    }

    public void CreateFolder()
    {
        Dictionary<string, string> allFolderNames = FileManager.GetFolderNames(currentFolderPath);

        if (allFolderNames.ContainsKey(inputField.text)) { PopupError.Popup("Folder already exists"); return;}

        string newPath = Path.Combine(currentFolderPath, inputField.text);
        Directory.CreateDirectory(newPath);

        GenerateFolderButton(new KeyValuePair<string, string>(inputField.text, newPath));
    }

    public void MoveFile()
    {
        if (_move)
        {
            _move = false;
            File.Move(_selectedFilePath, Path.Combine(currentFolderPath, Path.GetFileNameWithoutExtension(_selectedFilePath) + ".cworld"));
            
            GenerateCWorldButtons();
        }
        else
        {
            _move = true;
            _selectedFilePath = currentFilePath;
        }
    }
    
    public void GenerateFolderButton(KeyValuePair<string, string> folder)
    {
        int fileCount = Directory.GetFiles(folder.Value, "*.cworld").Length;
        
        if (!folderNames.Contains(folder.Key))
        {
            GameObject buttonContainer = new GameObject("Folder Container");
            folderButtons.Add(buttonContainer);
            buttonContainer.transform.SetParent(folderContent, false);
            var layoutGroup = buttonContainer.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.spacing = 2;
            
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer.transform);
            newButton.GetComponentInChildren<TMP_Text>().text = folder.Key;
            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => { currentFolderPath = folder.Value; GenerateCWorldButtons(); });

            if (fileCount == 0)
            {
                GameObject newDeleteButton = Instantiate(deleteButtonPrefab, buttonContainer.transform);
                Button deletebutton = newDeleteButton.GetComponent<Button>();
                deletebutton.onClick.AddListener(() => DeleteFolder(folder.Value, buttonContainer));
            }

            folderNames.Add(folder.Key);
        }
    }
    
    public void GenerateFileButton(string filePath)
    {
        string buttonName = Path.GetFileNameWithoutExtension(filePath);

        if (!fileNames.Contains(buttonName))
        {
            GameObject buttonContainer = new GameObject("File Container");
            fileButtons.Add(buttonContainer);
            buttonContainer.transform.SetParent(fileContent, false);
            var layoutGroup = buttonContainer.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.spacing = 2;
            
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer.transform);
            GameObject newDeleteButton = Instantiate(deleteButtonPrefab, buttonContainer.transform);
            
            newButton.GetComponentInChildren<TMP_Text>().text = buttonName;

            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => { currentFilePath = filePath; writer.DisplayContent(filePath); });
            
            Button deletebutton = newDeleteButton.GetComponent<Button>();
            deletebutton.onClick.AddListener(() => DeleteFile(filePath, buttonContainer));

            fileNames.Add(buttonName);
        }
    }
    
    public void DeleteFile(string filePath, GameObject container)
    {
        File.Delete(filePath);
        Destroy(container);
    }
    
    public void DeleteFolder(string folderPath, GameObject container)
    {
        Directory.Delete(folderPath);
        Destroy(container);
    }

    public void ClearInputField()
    {
        writer.Clear();
    }
    
    public void Save(string saveFile, string text)
    {
        if (!saveFile.Equals(""))
        {
            string filePath = Path.Combine(currentFolderPath, saveFile);
            File.WriteAllText(filePath, text);
        }
        
        //GenerateCWorldButtons();
    }
    
    public void SaveToPath(string filePath, string text)
    {
        if (!filePath.Equals(""))
        {
            File.WriteAllText(filePath, text);
        }
        
        GenerateCWorldButtons();
    }
}