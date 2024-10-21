using UnityEngine;
using UnityEngine.Windows;

public class UtilityManager : MonoBehaviour
{
    public static UtilityManager Instance;

    public Transform player;
    public Rigidbody rb;
    public CapsuleCollider cc;

    public LayerMask groundLayer;

    public Vector3 player_Half_Collider_Size = new Vector3(.4f, 1.5f, .4f);

    public Vector3 player_Collider_Center = new Vector3(0, -1, 0);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        cc = player.GetComponent<CapsuleCollider>();
    }



    public void SetDrag(float value)
    {
        rb.drag = value;
    }   
}

public static class InputManager
{
    public static PlayerInput inputs = PlayerInput.Instance;

    /**
    public static Vector2 MoveInput()
    {
        return Vector2.zero;
    }

    public static bool JumpInput()
    {
        return true;
    }

    public static bool ControlInput()
    {
        return true;
    }

    public static bool RightClickInput()
    {
        return true;
    }

    public static bool LeftClickInput()
    {
        return true;
    }

    public static bool CameraSwitchInput()
    {
        return true;
    }

    public static bool EscapeInput()
    {
        return true;
    }

    public static bool InventoryInput()
    {
        return true;
    }
    */
    
     public static Vector2 MoveInput()
    {
        return inputs.inputs.Move.ReadValue<Vector2>();
    }

    public static bool JumpInput()
    {
        return inputs.inputs.Jump.ReadValue<float>() > .5f;
    }

    public static bool ShiftInput()
    {
        return inputs.inputs.Shift.ReadValue<float>() > .5f;
    }

    public static bool ControlInput()
    {
        return inputs.inputs.Control.ReadValue<float>() > .5f;
    }

    public static bool RightClickInput()
    {
        return inputs.inputs.RightClick.ReadValue<float>() > .5f;
    }

    public static bool LeftClickInput()
    {
        return inputs.inputs.LeftClick.ReadValue<float>() > .5f;
    }

    public static bool CameraSwitchInput()
    {
        return inputs.inputs.SwitchCam.ReadValue<float>() > .5f;
    }

    public static bool EscapeInput()
    {
        return inputs.inputs.Escape.ReadValue<float>() > .5f;
    }

    public static bool InventoryInput()
    {
        return inputs.inputs.Inventory.ReadValue<float>() > .5f;
    }
}

public static class RayData
{
    public static readonly Vector3[] box_Collider_Ray_Positions = new Vector3[25]
    {
        new Vector3(  1f,  0f,  1f),
        new Vector3( .5f,  0f,  1f),
        new Vector3(  0f,  0f,  1f),
        new Vector3(-.5f,  0f,  1f),
        new Vector3( -1f,  0f,  1f),
        new Vector3(  1f,  0f, .5f),
        new Vector3( .5f,  0f, .5f),
        new Vector3(  0f,  0f, .5f),
        new Vector3(-.5f,  0f, .5f),
        new Vector3( -1f,  0f, .5f),
        new Vector3(  1f,  0f,  0f),
        new Vector3( .5f,  0f,  0f),
        new Vector3(  0f,  0f,  0f),
        new Vector3(-.5f,  0f,  0f),
        new Vector3( -1f,  0f,  0f),
        new Vector3(  1f,  0f,-.5f),
        new Vector3( .5f,  0f,-.5f),
        new Vector3(  0f,  0f,-.5f),
        new Vector3(-.5f,  0f,-.5f),
        new Vector3( -1f,  0f,-.5f),
        new Vector3(  1f,  0f, -1f),
        new Vector3( .5f,  0f, -1f),
        new Vector3(  0f,  0f, -1f),
        new Vector3(-.5f,  0f, -1f),
        new Vector3( -1f,  0f, -1f),
    };
}
