using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWorldFoliageNode
{
    public string name;
    public IntRangeNode lengthRange = new IntRangeNode(20, 30);
    
    public ITreeSampler sampler;
    
    public CWorldLinkNode trunk;
    public List<IDirection> directions = new List<IDirection>();

    public Vector3 forward = new Vector3(1, 0, 0);
    public Vector3 right = new Vector3(0, 0, 1);
    public Vector3 up = new Vector3(0, 1, 0);
    
    public IntRangeNode branchAmount = new IntRangeNode(4, 6);
    
    public FloatRangeNode thresholdRange = new FloatRangeNode(0.4f, 0.8f);
    public IRadius radius = new RadiusBase(1);
    public FloatRangeNode angleRange = new FloatRangeNode(0, 360);
    public IntRangeNode branchLengthRange = new IntRangeNode(10, 15);
    public FloatRangeNode verticalAngle = new FloatRangeNode(-30, 30);
    public Vector3 direction = new Vector3(1, 0, 0);
    
    public float branchOffset = 0.001f;
    
    public CWorldFoliageNode(string name)
    {
        this.name = name;
    }

    public void Generate(int x, int y, int z)
    {
        Generate(x, y, z, sampler.Sample());
    }
    
    public void Generate(int x, int y, int z, int height)
    {
        if (sampler == null || sampler.Ignore())
            return;
        
        //Trunk
        int trunkHeight = (int)NoiseUtils.LerpRange(lengthRange, NoiseUtils.GetNoiseAtPos(x, y, z));
        
        trunk.SetPositions(new Vector3Int(x, height, z), trunkHeight);
        trunk.GetPositions(out var a1, out var b1);
        
        Vector3 posA = new Vector3(a1.x, a1.y, a1.z);
        Vector3 posB = new Vector3(b1.x, b1.y, b1.z);
        
        foreach (var direction in directions)
        {
            direction.Rotate(this, ref posA, ref posB, x, y, z);
        }
        
        trunk.SetPositions(posA, posB);
        trunk.GenerateLink(1.2f);
        
        Vector3Int trunkOrigin = Vector3Int.RoundToInt(Vector3.Lerp(posA, posB, 1));
            
        var points = Chunk.GenerateStretchedSphere(17, 12, 17, trunkOrigin);
        var chunkDataCache = new Dictionary<Vector3Int, ChunkData>();

        foreach (var point in points)
        {
            if (NoiseUtils.Noise((float)((float)point.x + 0.001f), (float)((float)point.y + 0.001f), (float)((float)point.z + 0.001f)) > 0f)
                continue;
            
            Vector3Int chunkPosition = Chunk.GetChunkPosition(point);

            if (!chunkDataCache.TryGetValue(chunkPosition, out var chunkData))
            {
                if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out chunkData))
                {
                    chunkData = new ChunkData(chunkPosition);
                    chunkData.blocks ??= new Block[32768];

                    WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
                }

                chunkDataCache[chunkPosition] = chunkData;
            }
            
            if (chunkData == null)
                continue;

            Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
            int index = position.x + position.z * 32 + position.y * 1024;
            chunkData.blocks[index] ??= new Block(5, 0);
        }
        
        foreach (var chunkData in chunkDataCache)
        {
            WorldChunks.chunksToUpdate[chunkData.Key] = chunkData.Value;
        }
        
        
        
        //Branches
        int branchCount = NoiseUtils.LerpRange(branchAmount, NoiseUtils.GetNoiseAtPos(x, y, z));
        for (int i = 0; i < branchCount; i++)
        {
            trunk.GetPositions(out var a2, out var b2);

            float noise = NoiseUtils.GetNoiseAtPos(x + branchOffset, y + branchOffset, z + branchOffset);
            int length = NoiseUtils.LerpRange(branchLengthRange, noise);
            float angle = NoiseUtils.LerpRange(angleRange, noise);
            
            branchOffset += 13.853f;
            noise = NoiseUtils.GetNoiseAtPos(x + branchOffset, y + branchOffset, z + branchOffset);
            
            float vertical = NoiseUtils.LerpRange(verticalAngle, noise);
            
            Vector3 origin = Vector3.Lerp(a2, b2, NoiseUtils.LerpRange(thresholdRange, noise));
            Vector3 end = direction * length + origin;
            
            Debug.Log("Before: " + origin + " " + end + " " + length + " " + angle + " " + vertical);

            end = FoliageUtils.RotateBAroundAxis(new Vector3(0, 0, 1), ref origin, ref end, vertical);
            end = FoliageUtils.RotateBAroundAxis(Vector3.up, ref origin, ref end, angle);
            
            Debug.Log("After: " + origin + " " + end);
        
            branchOffset += 13.853f;

            CWorldLinkNode branch = new CWorldLinkNode("branch");
            branch.SetPositions(origin, end);
            branch.GenerateLink(1f);

            
            
            Vector3Int branchOrigin = Vector3Int.RoundToInt(Vector3.Lerp(origin, end, 0.75f));
            branchOrigin.y += 2;
            
            points = Chunk.GenerateStretchedSphere(12, 8, 12, branchOrigin);
            chunkDataCache = new Dictionary<Vector3Int, ChunkData>();

            foreach (var point in points)
            {
                if (NoiseUtils.Noise((float)((float)point.x + 0.001f), (float)((float)point.y + 0.001f), (float)((float)point.z + 0.001f)) > 0f)
                    continue;
                
                Vector3Int chunkPosition = Chunk.GetChunkPosition(point);

                if (!chunkDataCache.TryGetValue(chunkPosition, out var chunkData))
                {
                    if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out chunkData))
                    {
                        chunkData = new ChunkData(chunkPosition);
                        chunkData.blocks ??= new Block[32768];

                        WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
                    }

                    chunkDataCache[chunkPosition] = chunkData;
                }
            
                if (chunkData == null)
                    continue;

                Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
                int index = position.x + position.z * 32 + position.y * 1024;
                chunkData.blocks[index] ??= new Block(5, 0);
            }
        
            foreach (var chunkData in chunkDataCache)
            {
                WorldChunks.chunksToUpdate[chunkData.Key] = chunkData.Value;
            }
        }
    }
    
    public List<PointData> GenerateVE(int x, int y, int z)
    {
        List<PointData> totalPoints = new List<PointData>();

        if (sampler == null || sampler.Ignore())
            return null;
        
        
        
        //Trunk
        int trunkHeight = (int)NoiseUtils.LerpRange(lengthRange, NoiseUtils.GetNoiseAtPos(x, y, z));
        
        trunk.SetPositions(new Vector3Int(x, y, z), trunkHeight);
        trunk.GetPositions(out var a1, out var b1);
        
        Vector3 posA = new Vector3(a1.x, a1.y, a1.z);
        Vector3 posB = new Vector3(b1.x, b1.y, b1.z);
        
        foreach (var d in directions)
        {
            d.Rotate(this, ref posA, ref posB, x, y, z);
        }
        
        Vector3Int trunkOrigin = Vector3Int.RoundToInt(Vector3.Lerp(posA, posB, 1));
            
        var points = Chunk.GenerateStretchedSphere(17, 12, 17, trunkOrigin);

        foreach (var point in points)
        {
            if (NoiseUtils.Noise((float)((float)point.x + 0.001f), (float)((float)point.y + 0.001f), (float)((float)point.z + 0.001f)) > 0f)
                continue;
            
            totalPoints.Add(new PointData { point = point, id = 5 });
        }
        
        trunk.SetPositions(posA, posB);
        
        foreach (var p in trunk.GenerateLinkVE(1.2f))
        {
            Debug.Log(p);
            totalPoints.Add(new PointData { point = p, id = 4 });
        }
        
        
        
        //Branches
        int branchCount = NoiseUtils.LerpRange(branchAmount, NoiseUtils.GetNoiseAtPos(x, y, z));
        for (int i = 0; i < branchCount; i++)
        {
            trunk.GetPositions(out var a2, out var b2);

            float noise = NoiseUtils.GetNoiseAtPos(x + branchOffset, y + branchOffset, z + branchOffset);
            int length = NoiseUtils.LerpRange(branchLengthRange, noise);
            float angle = NoiseUtils.LerpRange(angleRange, noise);
            
            branchOffset += 13.853f;
            
            noise = NoiseUtils.GetNoiseAtPos(x + branchOffset, y + branchOffset, z + branchOffset);
            
            float vertical = NoiseUtils.LerpRange(verticalAngle, noise);
            
            Vector3 origin = Vector3.Lerp(a2, b2, NoiseUtils.LerpRange(thresholdRange, noise));
            Vector3 end = direction * length + origin;

            end = FoliageUtils.RotateBAroundAxis(new Vector3(0, 0, 1), ref origin, ref end, vertical);
            end = FoliageUtils.RotateBAroundAxis(Vector3.up, ref origin, ref end, angle);
        
            branchOffset += 13.853f;
            
            Vector3Int branchOrigin = Vector3Int.RoundToInt(Vector3.Lerp(origin, end, 0.75f));
            branchOrigin.y += 2;
            
            points = Chunk.GenerateStretchedSphere(12, 8, 12, branchOrigin);

            foreach (var point in points)
            {
                if (NoiseUtils.Noise((float)((float)point.x + 0.001f), (float)((float)point.y + 0.001f), (float)((float)point.z + 0.001f)) > 0f)
                    continue;
                
                totalPoints.Add(new PointData { point = point, id = 5 });
            }

            CWorldLinkNode branch = new CWorldLinkNode("branch");
            branch.SetPositions(origin, end);
            
            foreach (var p in branch.GenerateLinkVE(1f))
            {
                totalPoints.Add(new PointData { point = p, id = 4 });
            }
        }
                
        return totalPoints;
    }
}

public struct PointData
{
    public Vector3Int point;
    public int id;
}

public interface IFoliageSampler
{
    void Generate(CWorldFoliageNode foliageNode, int x, int y, int z, int height);
}

public class FoliageTrunk : IFoliageSampler
{
    public CWorldLinkNode link;
    public IRadius radius = new RadiusBase(1);
    public List<IDirection> directions = new List<IDirection>();
    
    public void Generate(CWorldFoliageNode foliageNode, int x, int y, int z, int height)
    {
        int trunkHeight = (int)NoiseUtils.LerpRange(foliageNode.lengthRange, NoiseUtils.GetNoiseAtPos(x, y, z));
        
        link.SetPositions(new Vector3Int(x, height, z), trunkHeight);
        link.GetPositions(out var a, out var b);
        
        Vector3 posA = new Vector3(a.x, a.y, a.z);
        Vector3 posB = new Vector3(b.x, b.y, b.z);
        
        foreach (var direction in directions)
        {
            direction.Rotate(foliageNode, ref posA, ref posB, x, y, z);
        }
        
        link.SetPositions(posA, posB);
        link.GenerateLink(radius.GetRadius(x, y, z));
    }
}

public class FoliageBranch : IFoliageSampler
{
    public CWorldLinkNode trunk;
    public CWorldLinkNode link;
    public FloatRangeNode thresholdRange = new FloatRangeNode(0, 1);
    public IRadius radius = new RadiusBase(1);
    public FloatRangeNode angleRange = new FloatRangeNode(0, 360);
    public IntRangeNode lengthRange = new IntRangeNode(5, 7);
    public IAngle verticalAngle = new AngleBase(0);
    public Vector3 direction = new Vector3(1, 0, 0);
    
    public void Generate(CWorldFoliageNode foliageNode, int x, int y, int z, int height)
    {
        trunk.GetPositions(out var a, out var b);

        float noise = NoiseUtils.GetNoiseAtPos(x + foliageNode.branchOffset, y + foliageNode.branchOffset, z + foliageNode.branchOffset);
        int length = NoiseUtils.LerpRange(lengthRange, noise);
        float angle = NoiseUtils.LerpRange(angleRange, noise);
        
        Vector3 origin = Vector3.Lerp(a, b, NoiseUtils.LerpRange(thresholdRange, noise));
        Vector3 end = direction * length + origin;

        end = FoliageUtils.RotateBAroundAxis(new Vector3(0, 0, 1), ref origin, ref end, angle);
        end = FoliageUtils.RotateBAroundAxis(Vector3.up, ref origin, ref end, angle);
        
        foliageNode.branchOffset += 13.853f;

        link = new CWorldLinkNode("branch");
        link.SetPositions(origin, end);
        link.GenerateLink(radius.GetRadius(x, y, z));
    }
}

public class FoliageLeaves : IFoliageSampler
{
    public void Generate(CWorldFoliageNode foliageNode, int x, int y, int z, int height)
    {
        
    }
}

public interface IRadius
{
    float GetRadius(int x, int y, int z);
}
    
public class RadiusBase : IRadius
{
    private readonly float _radius;
    
    public RadiusBase(float radius)
    {
        _radius = radius;
    }
        
    public float GetRadius(int x, int y, int z)
    {
        return _radius;
    }
}

public class RadiusRange : IRadius
{
    public FloatRangeNode range;
    
    public float GetRadius(int x, int y, int z)
    {
        return (int)NoiseUtils.LerpRange(range, NoiseUtils.GetNoiseAtPos(x, y, z));
    }
}



public interface IDirection
{
    void Move(ref Vector3 v, Vector3 offset, int x, int y, int z);
    void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z);
}

public class ForwardRotate : IDirection
{
    public IAngle angle = new AngleBase(0);
    
    public void Move(ref Vector3 v, Vector3 offset, int x, int y, int z)
    {
        
    }

    public void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z)
    {
        b = FoliageUtils.RotateBAroundAxis(foliageNode.forward, ref a, ref b, angle.GetAngle(x, y, z));
    }
}

public class BackwardsRotate : IDirection
{
    public IAngle angle = new AngleBase(0);
    
    public void Move(ref Vector3 v, Vector3 offset, int x, int y, int z)
    {
        
    }

    public void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z)
    {
        b = FoliageUtils.RotateBAroundAxis(-foliageNode.forward, ref a, ref b, angle.GetAngle(x, y, z));
    }
}

public class RightRotate : IDirection
{
    public IAngle angle = new AngleBase(0);
    
    public void Move(ref Vector3 v, Vector3 offset, int x, int y, int z)
    {
        
    }

    public void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z)
    {
        b = FoliageUtils.RotateBAroundAxis(foliageNode.right, ref a, ref b, angle.GetAngle(x, y, z));
    }
}

public class LeftRotate : IDirection
{
    public IAngle angle = new AngleBase(0);
    
    public void Move(ref Vector3 v, Vector3 offset, int x, int y, int z)
    {
        
    }

    public void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z)
    {
        b = FoliageUtils.RotateBAroundAxis(-foliageNode.right, ref a, ref b, angle.GetAngle(x, y, z));
    }
}

public class UpRotate : IDirection
{
    public IAngle angle = new AngleBase(0);
    
    public void Move(ref Vector3 v, Vector3 offset, int x, int y, int z)
    {
        
    }

    public void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z)
    {
        b = FoliageUtils.RotateBAroundAxis(foliageNode.up, ref a, ref b, angle.GetAngle(x, y, z));
    }
}

public class DownRotate : IDirection
{
    public IAngle angle = new AngleBase(0);
    
    public void Move(ref Vector3 v, Vector3 offset, int x, int y, int z)
    {
        
    }

    public void Rotate(CWorldFoliageNode foliageNode, ref Vector3 a, ref Vector3 b, int x, int y, int z)
    {
        b = FoliageUtils.RotateBAroundAxis(-foliageNode.up, ref a, ref b, angle.GetAngle(x, y, z));
    }
}



public static class FoliageUtils
{
    public static Vector3 RotateBAroundAxis(Vector3 axis, ref Vector3 a, ref Vector3 b, float angle)
    {
        Vector3 link = (b - a);
        
        Vector3 directionVector = link.normalized;
        float distance = link.magnitude;
        
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
        Vector3 rotated = rotation * directionVector;
        
        return a + rotated * distance;
    }
}


public interface IAngle
{
    float GetAngle(int x, int y, int z);
}

public class AngleBase : IAngle
{
    private readonly float _angle;
    
    public AngleBase(float angle)
    {
        _angle = angle;
    }
    
    public float GetAngle(int x, int y, int z)
    {
        return _angle;
    }
}

public class AngleRange : IAngle
{
    public FloatRangeNode range;
    
    public AngleRange(Vector2 range)
    {
        this.range = new FloatRangeNode(range.x, range.y);
    }
    
    public float GetAngle(int x, int y, int z)
    {
        return NoiseUtils.LerpRange(range, NoiseUtils.GetNoiseAtPos(x, y, z));
    }
}