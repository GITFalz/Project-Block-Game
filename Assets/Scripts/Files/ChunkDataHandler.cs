using System.IO;
using UnityEngine;

public static class ChunkDataHandler
{
    public static void WriteChunkDataOptimised(ChunkData chunkData)
    {
        string name = GetFileName(chunkData.position);
        string filePath = Path.Combine(FileManager.ChunkDataFolderPath, name);

        using BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));
        
        binaryWriter.Write(chunkData.position.x);
        binaryWriter.Write(chunkData.position.y);
        binaryWriter.Write(chunkData.position.z);

        if (chunkData.blocks == null)
            return;

        short count = 0;
        for (int i = 0; i < chunkData.blocks.Length; i++)
        {
            if (chunkData.blocks[i] == null)
            {
                count++;
            }
            else
            {
                if (count > 0)
                {
                    binaryWriter.Write((short)(0b1000000000000000 | count));
                    count = 0;
                }

                //Assuming blockData goes up to 32767 max
                binaryWriter.Write(chunkData.blocks[i].blockData);
            }
        }

        if (count > 0)
            binaryWriter.Write((short)(0b1000000000000000 | count));
    }
    
    public static void WriteChunkData(ChunkData chunkData)
    {
        string name = GetFileName(chunkData.position);
        string filePath = Path.Combine(FileManager.ChunkDataFolderPath, name);

        using BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));
        
        binaryWriter.Write(chunkData.position.x);
        binaryWriter.Write(chunkData.position.y);
        binaryWriter.Write(chunkData.position.z);

        if (chunkData.blocks == null)
            return;
        
        for (int i = 0; i < chunkData.blocks.Length; i++)
        {
            if (chunkData.blocks[i] == null)
            {
                binaryWriter.Write((short)(0));
            }
            else
            {
                binaryWriter.Write(chunkData.blocks[i].blockData);
            }
        }
    }

    public static bool ReadChunkDataOptimised(Vector3Int position, ChunkData chunkData)
    {
        string name = GetFileName(position);
        string filePath = Path.Combine(FileManager.ChunkDataFolderPath, name);
        
        Debug.Log(position);

        if (!File.Exists(filePath))
            return false;
        
        chunkData.blocks = new Block[32768];

        using BinaryReader binaryReader = new BinaryReader(File.Open(filePath, FileMode.Open));
        
        Vector3Int pos = new Vector3Int();
        pos.x = binaryReader.ReadInt32();
        pos.y = binaryReader.ReadInt32();
        pos.z = binaryReader.ReadInt32();

        int index = 0;
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
            short data = binaryReader.ReadInt16();
            
            if ((data & 0b1000000000000000) == 0)
            {
                chunkData.blocks[index] = new Block(data, 0);
                index++;
            }
            else
            {
                int count = data - 0b1000000000000000;

                for (int i = 0; i < count; i++)
                {
                    chunkData.blocks[index] = null;
                    index++;
                }
            }
        }

        return true;
    }
    
    public static bool ReadChunkData(Vector3Int position, ChunkData chunkData)
    {
        string name = GetFileName(position);
        string filePath = Path.Combine(FileManager.ChunkDataFolderPath, name);
        
        Debug.Log(position);

        if (!File.Exists(filePath))
            return false;
        
        chunkData.blocks = new Block[32768];

        using BinaryReader binaryReader = new BinaryReader(File.Open(filePath, FileMode.Open));
        
        Vector3Int pos = new Vector3Int();
        pos.x = binaryReader.ReadInt32();
        pos.y = binaryReader.ReadInt32();
        pos.z = binaryReader.ReadInt32();

        int index = 0;
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
            short data = binaryReader.ReadInt16();
            
            if (data != 0)
            {
                chunkData.blocks[index] = new Block(data, 0);
            }
            else
            {
                chunkData.blocks[index] = null;
            }
            index++;
        }

        return true;
    }

    private static string GetFileName(Vector3Int pos)
    {
        return $"{pos.x},{pos.y},{pos.z}.chunkdata";
    }
    
    private static string DataToString(short data)
    {
        string d = "";
        
        for (int i = 0; i < 16; i++)
        {
            d += (data & ((15 - i) << i)) != 0 ? 1 : 0;
        }

        return d;
    }
}