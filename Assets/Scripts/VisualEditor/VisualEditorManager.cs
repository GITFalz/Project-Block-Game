using System.Collections.Generic;
using UnityEngine;

public class VisualEditorManager : MonoBehaviour
{
    public Vector3Int modelSize;
    
    public CWorldFoliageNode foliageNode;
    public VisualEditorDisplay visualEditorDisplay;
    public WMWriter writer;
    
    public async void GenerateFoliage(string content)
    {
        await writer.LoadContent(content);
            
        foliageNode = new CWorldFoliageNode(CWorldFoliageManager.name);
        
        foliageNode.sampler = new TreeBasic();
        foliageNode.trunk = new CWorldLinkNode(CWorldFoliageManager.trunkName);
        
        foreach (var direction in CWorldFoliageManager.directions)
        {
            switch (direction.direction)
            {
                case "forward":
                    foliageNode.directions.Add(new ForwardRotate { angle = new AngleRange(direction.values) });
                    break;
                case "backward":
                    foliageNode.directions.Add(new BackwardsRotate { angle = new AngleRange(direction.values) });
                    break;
                case "right":
                    foliageNode.directions.Add(new RightRotate { angle = new AngleRange(direction.values) });
                    break;
                case "left":
                    foliageNode.directions.Add(new LeftRotate { angle = new AngleRange(direction.values) });
                    break;
                case "up":
                    foliageNode.directions.Add(new UpRotate { angle = new AngleRange(direction.values) });
                    break;
                case "down":
                    foliageNode.directions.Add(new DownRotate { angle = new AngleRange(direction.values) });
                    break;
            }
        }
        
        foliageNode.lengthRange = new IntRangeNode(CWorldFoliageManager.lengthRange);
        foliageNode.branchAmount = new IntRangeNode(CWorldFoliageManager.branchAmount);
        foliageNode.branchLengthRange = new IntRangeNode(CWorldFoliageManager.branchLengthRange);
        foliageNode.angleRange = new FloatRangeNode(CWorldFoliageManager.angleRange);
        foliageNode.thresholdRange = new FloatRangeNode(CWorldFoliageManager.thresholdRange);
        foliageNode.verticalAngle = new FloatRangeNode(CWorldFoliageManager.verticalAngle);

        List<PointData> points = foliageNode.GenerateVE(0, 0, 0);

        int minX = 0, maxX = 0, minY = 0, maxY = 0, minZ = 0, maxZ = 0, x = 0, y = 0, z = 0;

        bool init = true;
        foreach (var point in points)
        {
            if (init)
            {
                minX = point.point.x;
                maxX = point.point.x;
                
                minY = point.point.y;
                maxY = point.point.y;
                
                minZ = point.point.z;
                maxZ = point.point.z;
                
                init = false;
            }
            
            if (point.point.x < minX)
                minX = point.point.x;
            if (point.point.x > maxX)
                maxX = point.point.x;
            
            if (point.point.y < minY)
                minY = point.point.y;
            if (point.point.y > maxY)
                maxY = point.point.y;
            
            if (point.point.z < minZ)
                minZ = point.point.z;
            if (point.point.z > maxZ)
                maxZ = point.point.z;
        }

        x = (maxX - minX) + 1; 
        y = (maxY - minY) + 1;
        z = (maxZ - minZ) + 1;
        
        Block[] blocks = new Block[x * z * y];

        foreach (var point in points)
        {
            int index = (point.point.x - minX) + x * ((point.point.z - minZ) + z * (point.point.y - minY));
            blocks[index] = new Block((short)point.id, 0);
        }
        
        MeshData meshData = new MeshData();
        VisualEditorGenerator.GenerateOcclusion(blocks, x, y, z);
        VisualEditorGenerator.GenerateMesh(meshData, blocks, x, y, z);
        visualEditorDisplay.RenderMesh(meshData);
        visualEditorDisplay.CenterModel(new Vector3Int(x, y, z));
        
        modelSize = new Vector3Int(x, y, z);
    }
}