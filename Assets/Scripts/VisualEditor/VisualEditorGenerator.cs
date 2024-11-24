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
        
        for (int y = 0; y < X; y++)
        {
            for (int z = 0; z < Y; z++)
            {
                for (int x = 0; x < Z; x++)
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
                                
                                int i = index;
                                int loop = VoxelData.FirstLoopVe[side](y, z, Y, Z);
                                int height = 1;
                                int width = 1;
                                while (loop > 0)
                                {
                                    i += VoxelData.FirstOffsetVe[side](X, Z);
                                    if (blocks[i] == null)
                                        break;

                                    if (((blocks[i].check >> side) & 1) != 0 ||
                                        ((blocks[i].occlusion >> side) & 1) != 0 ||
                                        blocks[i].blockData != block.blockData)
                                        break;

                                    blocks[i].check |= (byte)(1 << side);

                                    height++;
                                    loop--;
                                }

                                i = index;
                                loop = VoxelData.SecondLoopVe[side](x, z, X, Z);
                                
                                bool quit = false;
                                
                                while (loop > 0)
                                {
                                    i += VoxelData.SecondOffsetVe[side](X);
                                    int up = i;
                                    
                                    for (int j = 0; j < height; j++)
                                    {
                                        if (blocks[up] == null)
                                        {
                                            quit = true;
                                            break;
                                        }

                                        if (((blocks[up].check >> side) & 1) != 0 ||
                                            ((blocks[up].occlusion >> side) & 1) != 0 ||
                                            blocks[up].blockData != block.blockData)
                                        {
                                            quit = true;
                                            break;
                                        }
                                        
                                        up += VoxelData.FirstOffsetVe[side](X, Z);
                                    }
                                    
                                    if (quit) break;
                                    
                                    up = i;
                                    
                                    for (int j = 0; j < height; j++) {
                                        blocks[up].check |= (byte)(1 << side);
                                        up += VoxelData.FirstOffsetVe[side](X, Z);
                                    }

                                    width++;
                                    loop--;
                                }

                                Vector3[] positions = VoxelData.PositionOffset[side](width, height);
                                Vector3 position = new Vector3(x, y, z);

                                int id = ids[side];
                                
                                meshData.uvs.Add(new Vector3(0, 0, id));
                                meshData.uvs.Add(new Vector3(0, height, id));
                                meshData.uvs.Add(new Vector3(width, height, id));
                                meshData.uvs.Add(new Vector3(width, 0, id));
                                
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