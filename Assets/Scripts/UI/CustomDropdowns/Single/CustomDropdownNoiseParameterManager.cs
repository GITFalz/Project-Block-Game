using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomDropdownNoiseParameterManager : CustomSingleDropdownAbstractManager
{
    [Header("Properties")]
    public List<string> options = new List<string>();
    
    public override void Init(CustomUICollectionManager collectionManager)
    {
        _collectionsManager = collectionManager;
        
        Dropdown1 = transform.Find("Dropdown").Find("Dropdown1").GetComponent<TMP_Dropdown>();
        Dropdown1.ClearOptions();
        
        foreach (string option in options)
        {
            Dropdown1.options.Add(new TMP_Dropdown.OptionData(option));
        }
        
        input.Init(collectionManager);
        
        _rectTransform = transform.GetComponent<RectTransform>();
        _height = _rectTransform.rect.height;
    }
    
    public override string ToCWorld()
    {
        string dropdownValue = Dropdown1.options[Dropdown1.value].text;
        string inputValue = (input.doName ? $"\n" : "") + input.ToCWorld();
        
        return $"{dropdownValue} {inputValue}";
    }
}