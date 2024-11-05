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
    public CWorldMapManager worldMapManager;
    public CWorldModifierManager worldModifierManager;

    public string currentName = "";
    public string currentBiomeName = "";
    public string currentBlockName = "";
    public string currentModifierName = "";
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

        worldMapManager = new CWorldMapManager();
        
        worldModifierManager = new CWorldModifierManager();

        this.import = import;
    }
    
    public string[] InitLines(string content)
    {
        index = 0;
        currentName = "";
        currentBiomeName = "";
        currentBlockName = "";
        currentModifierName = "";
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

    public string CurrentLine()
    {
        return lines[index];
    }

    public string NextLine(int i = 1)
    {
        return index + i >= lines.Length ? "" : lines[index + i];
    }

    public void Inc(int i = 1)
    {
        index += i;
    }
}