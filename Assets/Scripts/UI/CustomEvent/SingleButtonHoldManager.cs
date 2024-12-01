using UnityEngine;
using UnityEngine.UI;

public class SingleButtonHoldManager : MonoBehaviour
{
    public OnButtonHold buttonA;
    
    private Button _bA;
    private bool _a;
    
    private void Start()
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
}