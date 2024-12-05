using UnityEngine;
using UnityEngine.UI;

public class SingleButtonHoldManager : CustomUI
{
    public OnButtonHold buttonA;
    
    private Button _bA;
    private bool _a;
    
    public override void Init(CustomUICollectionManager collectionManager)
    {
        _bA = buttonA.GetComponent<Button>();
        _bA.onClick.AddListener(() => { _a = true; });
    }
    
    public bool IsClicking()
    {
        if (!_a) return false;
        
        _a = false;
        return true;
    }
    
    public bool IsHolding()
    {
        return buttonA.IsHolding();
    }
    
    public override float Align(Vector3 position)
    {
        transform.position = position;
        return transform.GetComponent<RectTransform>().rect.height;
    }
    
    public override string ToCWorld()
    {
        return "";
    } 
}