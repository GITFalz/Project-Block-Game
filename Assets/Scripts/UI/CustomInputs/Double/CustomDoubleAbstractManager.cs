using TMPro;
using UnityEngine;

public abstract class CustomDoubleAbstractManager : CustomInputAbstractManager
{
    protected TMP_InputField _fieldA;
    protected TMP_InputField _fieldB;
    
    [Header("Buttons")]
    public ButtonHold buttonHoldBoth;
    public ButtonHold buttonHoldA;
    public ButtonHold buttonHoldB;
    

    protected override void InitExtra(CustomUICollectionManager collectionManager)
    {
        buttonHoldBoth.action = SetBoth;
        buttonHoldA.action = SetA;
        buttonHoldB.action = SetB;
        
        _text = transform.Find("Panel").Find("Text").GetComponent<TMP_Text>();
        _text.text = name.type;
        
        _fieldA = transform.Find("Panel").Find("Input1").GetComponent<TMP_InputField>();
        _fieldB = transform.Find("Panel").Find("Input2").GetComponent<TMP_InputField>();
        
        InitValues();
    }

    public void SetBoth()
    {
        SetHoldBoth(MouseData.GetMouseX() * GetSlideSpeed());
        InitValues();
    }
    
    public void SetA()
    {
        SetHoldA(MouseData.GetMouseX() * GetSlideSpeed());
        InitValues();
    }
    
    public void SetB()
    {
        SetHoldB(MouseData.GetMouseX() * GetSlideSpeed());
        InitValues();
    }

    protected abstract void SetHoldBoth(float x);
    protected abstract void SetHoldA(float x);
    protected abstract void SetHoldB(float x);
    
    protected abstract void InitValues();

    public abstract override string ToCWorld();
}