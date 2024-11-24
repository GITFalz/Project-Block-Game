
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;
    public PlayerController inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inputActions = new PlayerController();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    public Vector2 MoveInput()
    {
        return inputActions.Controls.Move.ReadValue<Vector2>();
    }

    public bool JumpInput()
    {
        return inputActions.Controls.Jump.ReadValue<float>() > .5f;
    }

    public bool ShiftInput()
    {
        return inputActions.Controls.Shift.ReadValue<float>() > .5f;
    }

    public bool ControlInput()
    {
        return inputActions.Controls.Control.ReadValue<float>() > .5f;
    }
    
    public bool EInput()
    {
        return inputActions.Controls.E.ReadValue<float>() > .5f;
    }
    
    public bool CInput()
    {
        return inputActions.Controls.C.ReadValue<float>() > .5f;
    }
    
    public bool DInput()
    {
        return inputActions.Controls.D.ReadValue<float>() > .5f;
    }
    
    public bool PInput()
    {
        return inputActions.Controls.P.ReadValue<float>() > .5f;
    }

    public bool AInput()
    {
        return inputActions.Controls.A.ReadValue<float>() > .5f;
    }

    public bool RightClickInput()
    {
        return inputActions.Controls.RightClick.ReadValue<float>() > .5f;
    }

    public bool LeftClickInput()
    {
        return inputActions.Controls.LeftClick.ReadValue<float>() > .5f;
    }

    public bool CameraSwitchInput()
    {
        return inputActions.Controls.SwitchCam.ReadValue<float>() > .5f;
    }

    public bool EscapeInput()
    {
        return inputActions.Controls.Escape.ReadValue<float>() > .5f;
    }

    public bool InventoryInput()
    {
        return inputActions.Controls.Inventory.ReadValue<float>() > .5f;
    }
}
