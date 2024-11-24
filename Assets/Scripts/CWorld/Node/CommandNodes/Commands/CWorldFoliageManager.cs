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
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> Labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref name) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> Settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "trunk", async (w) => await w.On_Settings(TrunkSettings) },
        { "branch", async (w) => await w.On_Settings(BranchSettings) },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> TrunkSettings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", async (w) =>
            {
                w.GetNextValue(out string value);
                w.Increment();
                samplerName = value;
                return 0;
            }
        },
        {
            "link", async (w) =>
            {
                w.GetNextValue(out string value);
                w.Increment();
                trunkName = value;
                return 0;
            }
        },
        { 
            "radius-single", async (w) =>
            {
                if (await w.GetNextFloat(out float value) == -1)
                    return await w.Error("radius must be a float");

                
                return 0;
            }
        },
        { 
            "radius-range", async (w) =>
            {
                if (await w.GetNextFloat(out float value) == -1)
                    return await w.Error("radius must be a float");

                
                return 0;
            }
        },
        { 
            "length", async (w) =>
            {
                if (await w.GetNext2Ints(out var ints) == -1)
                    return await w.Error("length must be two ints");
                lengthRange = new Vector2Int(ints.x, ints.y);
                return 0;
            }
        },
        { "direction", async (w) => await w.On_Settings(TrunkDirectionSettings) },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> TrunkDirectionSettings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { 
            "angle", async (w) =>
            {
                w.GetNextValue(out var vector);
                
                if (await w.GetNext2Floats(out var floats) == -1)
                {
                    w.Increment(-4);
                    if (await w.GetNextFloat(out var value) == -1)
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
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> BranchSettings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { 
            "range", async (w) =>
            {
                if (await w.GetNext2Ints(out var ints) == -1)
                    return await w.Error("length must be two ints");
                branchAmount = new Vector2Int(ints.x, ints.y);
                return 0;
            }
        },
        { 
            "length", async (w) =>
            {
                if (await w.GetNext2Ints(out var ints) == -1)
                    return await w.Error("length must be two ints");
                branchLengthRange = new Vector2Int(ints.x, ints.y);
                return 0;
            }
        },
        { 
            "angle", async (w) =>
            {
                if (await w.GetNext2Floats(out var floats) == -1)
                    return await w.Error("length must be two ints");
                angleRange = new Vector2(floats.x, floats.y);
                return 0;
            }
        },
        { 
            "threshold", async (w) =>
            {
                if (await w.GetNext2Floats(out var floats) == -1)
                    return await w.Error("length must be two ints");
                thresholdRange = new Vector2(floats.x, floats.y);
                return 0;
            }
        },
        { 
            "z", async (w) =>
            {
                if (await w.GetNext2Floats(out var floats) == -1)
                    return await w.Error("length must be two ints");
                verticalAngle = new Vector2(floats.x, floats.y);
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
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