using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject
{
    public List<WorldConfigData> worldData = new();
}

[System.Serializable]
public class WorldConfigData
{
    public string worldName;
    public PlayerConfigData playerData;
    public List<CWorldDataHandler> dataHandlers;
    public Dictionary<int, CWorldBlock> Blocks;
}

[System.Serializable]
public class PlayerConfigData
{
    public Vector3 position;
    public Vector3 rotation;
}