using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block")]
public class BlockSO : ScriptableObject
{
    public short index;
    public Uvs blockUvs = defaultUvs;

    public static Uvs defaultUvs => new Uvs();
    
    [Serializable]
    public class Uvs
    {
        public int[] uvIndex;

        public Uvs()
        {
            uvIndex = new int[6];
        }
    }
}