using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomDropdownOverrideSamplesManager : CustomDoubleDropdownAbstractManager
{
    [Header("Properties")]
    public List<string> options1 = new List<string>();
    public List<string> options2 = new List<string>();
    
    private SampleCollectionManager sampleCollectionManager;

    public override void Init(CustomUICollectionManager collectionManager)
    {
        sampleCollectionManager = GameObject.Find("Managers").transform.Find("Collection").GetComponent<SampleCollectionManager>();
        
        sampleCollectionManager.AddSample(collectionManager.currentNewCollectionName, this);
        
        Dropdown1 = transform.Find("Dropdown").Find("Dropdown1").GetComponent<TMP_Dropdown>();
        Dropdown2 = transform.Find("Dropdown").Find("Dropdown2").GetComponent<TMP_Dropdown>();
        Dropdown1.ClearOptions();
        Dropdown2.ClearOptions();

        foreach (string option in options1)
        {
            Dropdown1.options.Add(new TMP_Dropdown.OptionData(option));
        }

        foreach (string option in options2)
        {
            Dropdown2.options.Add(new TMP_Dropdown.OptionData(option));
        }

        if (input != null)
            input.Init(collectionManager);
    }

    public void UpdateSamples()
    {
        string value = Dropdown2.options[Dropdown2.value].text;
        
        Dropdown2.ClearOptions();
        foreach (string option in options2)
        {
            Dropdown2.options.Add(new TMP_Dropdown.OptionData(option));
        }
        
        Dropdown2.value = Dropdown2.options.FindIndex(option => option.text == value);
    }

    public override string ToCWorld()
    {
        string dropdownValue1 = Dropdown1.options[Dropdown1.value].text;
        string dropdownValue2 = Dropdown2.options[Dropdown2.value].text;

        return $"{dropdownValue1} {dropdownValue2}";
    }
}