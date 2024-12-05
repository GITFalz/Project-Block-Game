using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CustomInputAbstractManager : CustomUI
{
    [Header("Properties")] 
    public TypeOrText name;
    public bool doName = true;
    public float height = 30;
    public SliderSpeed sliderSpeed = SliderSpeed.Normal;
    
    [Header("Check")]
    public Button checkButton;
    public bool isChecked = false;
    
    private TMP_Text _buttonText;
    protected TMP_Text _text;
    
    public void SetChecked()
    {
        isChecked = !isChecked;
        _buttonText.text = isChecked ? "V" : "X";
    }

    public override void Init(CustomUICollectionManager collectionManager)
    {
        _buttonText = checkButton.transform.Find("Text").GetComponent<TMP_Text>();
        _buttonText.text = isChecked ? "V" : "X";
        
        checkButton.onClick.AddListener(SetChecked);
        
        InitExtra(collectionManager);
    }

    protected abstract void InitExtra(CustomUICollectionManager collectionManager);

    public override float Align(Vector3 position)
    {
        Debug.Log("Double: " + position);
        
        transform.position = position;
        return height;
    }

    public override abstract string ToCWorld();

    protected float GetSlideSpeed()
    {
        return Speed[sliderSpeed];
    }

    private static readonly Dictionary<SliderSpeed, float> Speed = new Dictionary<SliderSpeed, float>()
    {
        {SliderSpeed.Slow, 0.2f},
        {SliderSpeed.Normal, 1f},
        {SliderSpeed.Fast, 2f}
    };
}

public enum SliderSpeed
{
    Slow,
    Normal,
    Fast
}