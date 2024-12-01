using TMPro;
using UnityEngine;

public class CustomSingleIntManager : MonoBehaviour
{
    [Header("Properties")] public string name;
    
    private TMP_Text _text;
    private TMP_InputField _fieldA;

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _text.text = name;
        
        _fieldA = transform.Find("Input1").GetComponent<TMP_InputField>();
    }
}
