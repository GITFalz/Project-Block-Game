
using System;
using UnityEngine;

[System.Serializable]
public abstract class CustomUIBase : MonoBehaviour
{
    public int count;
    public GameObject go;
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (count > 0)
        {
            count--;
            go.SetActive(false);
            go.SetActive(true);
        }
    }

    public abstract void Init();
    public abstract string ParameterToText();
}