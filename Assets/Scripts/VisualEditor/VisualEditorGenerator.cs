using UnityEngine;

public static class VisualEditorGenerator
{
    public static void GenerateOcclusion(Block[] blocks, int X, int Y, int Z)
    {
        int index = 0;
        for (int y = 0; y < Y; y++)
        {
            for (int z = 0; z < Z; z++)
            {
                for (int x = 0; x < X; x++)
                {
                    if (blocks[index] != null)
                    {
                        byte occlusion = 0;
                            
                        for (int i = 0; i < 6; i++)
                        {
                            if (VoxelData.InBounds(x, y, z, i, X, Y, Z) && blocks[index + VoxelData.IndexOffsetVE[i](X, Z)] != null)
                                occlusion |= VoxelData.ShiftPosition[i];
                        }
                            
                        blocks[index].occlusion = occlusion;
                    }
                        
                    index++;
                }
            }
        }
    }
    public static void GenerateMesh(MeshData meshData, Block[] blocks, int X, int Y, int Z)
    {
        int index = 0;
        
        for (int y = 0; y < Y; y++)
        {
            for (int z = 0; z < Z; z++)
            {
                for (int x = 0; x < X; x++)
                {
                    Block block = blocks[index];
                    
                    if (block != null)
                    {
                        int[] ids = BlockManager.GetBlock(block.blockData).GetUVs();
                        
                        for (int side = 0; side < 6; side++)
                        {
                            if (((block.check >> side) & 1) == 0 && ((block.occlusion >> side) & 1) == 0)
                            {
                                block.check |= (byte)(1 << side);
                                
                                for (int tris = 0; tris < 6; tris++)
                                {
                                    meshData.tris.Add(VoxelData.TrisIndexTable[tris] + meshData.Count());
                                }
                                
                                

                                Vector3[] positions = VoxelData.PositionOffset[side](1, 1);
                                Vector3 position = new Vector3(x, y, z);

                                int id = ids[side];
                                
                                meshData.uvs.Add(new Vector3(0, 0, id));
                                meshData.uvs.Add(new Vector3(0, 1, id));
                                meshData.uvs.Add(new Vector3(1, 1, id));
                                meshData.uvs.Add(new Vector3(1, 0, id));
                                
                                meshData.verts.Add(position + positions[0]);
                                meshData.verts.Add(position + positions[1]);
                                meshData.verts.Add(position + positions[2]);
                                meshData.verts.Add(position + positions[3]);
                            }
                        }
                    }
                    
                    index++;
                }
            }
        }
    }
}