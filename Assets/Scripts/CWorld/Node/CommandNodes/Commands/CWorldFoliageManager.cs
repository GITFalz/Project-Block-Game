using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldFoliageManager
{
    public static string name;
    
    public static string samplerName;
    public static string trunkName;
    public static List<FoliageDirectionData> directions = new List<FoliageDirectionData>();

    public static Vector2Int lengthRange = new Vector2Int(20, 30);
    public static Vector2Int branchAmount = new Vector2Int(4, 6);
    public static Vector2Int branchLengthRange = new Vector2Int(10, 15);
    
    public static Vector2 thresholdRange = new Vector2(0.4f, 0.8f);
    public static Vector2 angleRange = new Vector2(0, 360);
    public static Vector2 verticalAngle = new Vector2(-30, 30);
    
    public static Vector3 direction = new Vector3(1, 0, 0);
    
    public static IRadius radius = new RadiusBase(1);
    
    public static Dictionary<string, Func<Task<int>>> Labels = new Dictionary<string, Func<Task<int>>>()
    {
        { "(", () => Increment(1, 0) },
        { "name", () => CWorldNodesManager.On_Name(ref name) },
        { ")", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> Settings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { "trunk", async () => await CWorldNodesManager.On_Settings(TrunkSettings) },
        { "branch", async () => await CWorldNodesManager.On_Settings(BranchSettings) },
        { "}", () => Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> TrunkSettings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        {
            "sample", () =>
            {
                CWorldCommandManager.GetNextValue(out string value);
                Increment();
                samplerName = value;
                return Task.FromResult(0);
            }
        },
        {
            "link", () =>
            {
                CWorldCommandManager.GetNextValue(out string value);
                Increment();
                trunkName = value;
                return Task.FromResult(0);
            }
        },
        { 
            "radius-single", async () =>
            {
                if (await CWorldCommandManager.GetNextFloat(out float value) == -1)
                    return await Error("radius must be a float");

                
                return 0;
            }
        },
        { 
            "radius-range", async () =>
            {
                if (await CWorldCommandManager.GetNextFloat(out float value) == -1)
                    return await Error("radius must be a float");

                
                return 0;
            }
        },
        { 
            "length", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out var ints) == -1)
                    return await Error("length must be two ints");
                lengthRange = new Vector2Int(ints.x, ints.y);
                return 0;
            }
        },
        { "direction", async () => await CWorldNodesManager.On_Settings(TrunkDirectionSettings) },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> TrunkDirectionSettings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { 
            "angle", async () =>
            {
                CWorldCommandManager.GetNextValue(out var vector);
                
                if (await CWorldCommandManager.GetNext2Floats(out var floats) == -1)
                {
                    Increment(-4);
                    if (await CWorldCommandManager.GetNextFloat(out var value) == -1)
                    {
                        return -1;
                    }
                }
                else
                {
                    switch (vector)
                    {
                        case "forward":
                            directions.Add(new FoliageDirectionData("forward", floats));
                            break;
                        case "backward":
                            directions.Add(new FoliageDirectionData("backward", floats));
                            break;
                        case "right":
                            directions.Add(new FoliageDirectionData("right", floats));
                            break;
                        case "left":
                            directions.Add(new FoliageDirectionData("left", floats));
                            break;
                        case "up":
                            directions.Add(new FoliageDirectionData("up", floats));
                            break;
                        case "down":
                            directions.Add(new FoliageDirectionData("down", floats));
                            break;
                        default:
                            Console.Log(vector + " is not a valid vector");
                            return -1;
                    }
                }
                

                return 0;
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> BranchSettings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { 
            "range", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out var ints) == -1)
                    return await Error("length must be two ints");
                branchAmount = new Vector2Int(ints.x, ints.y);
                return 0;
            }
        },
        { 
            "length", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out var ints) == -1)
                    return await Error("length must be two ints");
                branchLengthRange = new Vector2Int(ints.x, ints.y);
                return 0;
            }
        },
        { 
            "angle", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out var floats) == -1)
                    return await Error("length must be two ints");
                angleRange = new Vector2(floats.x, floats.y);
                return 0;
            }
        },
        { 
            "threshold", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out var floats) == -1)
                    return await Error("length must be two ints");
                thresholdRange = new Vector2(floats.x, floats.y);
                return 0;
            }
        },
        { 
            "z", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out var floats) == -1)
                    return await Error("length must be two ints");
                verticalAngle = new Vector2(floats.x, floats.y);
                return 0;
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    private static void Increment(int i = 1)
    {
        CWorldCommandManager.Increment(i);
    }

    private static async Task<int> Increment(int i, int result)
    {
        return await CWorldCommandManager.Increment(i, result);
    }
    
    private static async Task<int> Error(string message)
    {
        return await Console.LogErrorAsync(message);
    }
}

public struct FoliageDirectionData
{
    public string direction;
    public Vector2 values;
    
    public FoliageDirectionData(string direction, Vector2 values)
    {
        this.direction = direction;
        this.values = values;
    }
}