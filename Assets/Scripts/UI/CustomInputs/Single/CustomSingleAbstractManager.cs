using TMPro;
using UnityEngine;

public abstract class CustomSingleAbstractManager : CustomInputAbstractManager
{
    protected TMP_InputField _fieldA;
    
    [Header("Buttons")]
    public ButtonHold buttonHoldA;

    protected override void InitExtra(CustomUICollectionManager collectionManager)
    {
        buttonHoldA.action = SetA;
        
        _text = transform.Find("Panel").Find("Text").GetComponent<TMP_Text>();
        _text.text = name.type;
    
        _fieldA = transform.Find("Panel").Find("Input1").GetComponent<TMP_InputField>();
        
        InitValues();
    }
    
    public void SetA()
    {
        SetHoldA(MouseData.GetMouseX());
        InitValues();
    }
    
    protected abstract void SetHoldA(float x);

    protected abstract void InitValues();

    public abstract override string ToCWorld();
}