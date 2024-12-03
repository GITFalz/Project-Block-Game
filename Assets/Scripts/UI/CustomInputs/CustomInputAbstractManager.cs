using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CustomInputAbstractManager : MonoBehaviour, I_CustomUi
{
    [Header("Properties")] 
    public TypeOrText name;
    public bool doName = true;
    public float height = 30;
    public bool sliderSpeed = false;
    
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

    public void Init(CustomUICollectionManager collectionManager)
    {
        _buttonText = checkButton.transform.Find("Text").GetComponent<TMP_Text>();
        _buttonText.text = isChecked ? "V" : "X";
        
        checkButton.onClick.AddListener(SetChecked);
        
        InitExtra(collectionManager);
    }

    protected abstract void InitExtra(CustomUICollectionManager collectionManager);

    public float Align(Vector3 position)
    {
        Debug.Log("Double: " + position);
        
        transform.position = position;
        return height;
    }

    public abstract string ToCWorld();
}

public enum SliderSpeed
{
    Slow,
    Medium,
    Fast
}