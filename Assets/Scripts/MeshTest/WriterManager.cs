using System;
using System.Text;
using System.Text.RegularExpressions;

public class WriterManager
{
    public string[] lines;
    public int index;
    public char[] charactersToReplace = { '(', ')', '=', '{', '}', ',', ':', '/'};

    public string saveFile = "";
    public string savePath = "";

    public string fileContent;

    public CWAOperatorNode currentNode;
    public CWorldSampleManager worldSampleManager;
    public CWorldBiomeManager worldBiomeManager;
    public CWorldBlockManager worldBlockManager;

    public string currentName = "";
    public string currentBiomeName = "";
    public string currentBlockName = "";
    public string currentType = "";

    public string displayName = "";

    public bool import;
    
    public WriterManager(WMWriter writer, bool import)
    {
        worldSampleManager = new CWorldSampleManager();
        worldSampleManager.writer = writer;

        worldBiomeManager = new CWorldBiomeManager();
        worldBiomeManager.writer = writer;

        worldBlockManager = new CWorldBlockManager();
        worldBlockManager.writer = writer;

        this.import = import;
    }
    
    public string[] InitLines(string content)
    {
        index = 0;
        currentName = "";
        currentBiomeName = "";
        currentBlockName = "";
        currentType = "";
        displayName = "";
        saveFile = "";

        fileContent = content;
        
        content = Regex.Replace(content, @"\u200B", "").Trim();

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < content.Length; i++)
        {
            if (i < content.Length - 1 && content[i] == '/' && content[i + 1] == '/') {
                result.Append(" // ");
                i++;
            }
            else if (Array.Exists(charactersToReplace, element => element == content[i]))
                result.Append($" {content[i]} ");
            else
                result.Append(content[i]);
        }

        content = result.ToString().Trim();
        
        lines = content.Split(new[] { '\n','\t', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        return lines;
    }
}