using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Chunk Settings")]
public class ChunkSettingsSO : ScriptableObject
{
    public float density = .5f;
    public bool showNoise = false;
}
