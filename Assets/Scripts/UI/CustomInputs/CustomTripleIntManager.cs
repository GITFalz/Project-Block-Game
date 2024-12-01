using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomTripleIntManager : MonoBehaviour
{
    [Header("Properties")] public string name;
    
    private TMP_Text _text;
    private TMP_InputField _fieldA;
    private TMP_InputField _fieldB;
    private TMP_InputField _fieldC;

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _text.text = name;
        
        _fieldA = transform.Find("Input1").GetComponent<TMP_InputField>();
        _fieldB = transform.Find("Input2").GetComponent<TMP_InputField>();
        _fieldC = transform.Find("Input3").GetComponent<TMP_InputField>();
    }
}
