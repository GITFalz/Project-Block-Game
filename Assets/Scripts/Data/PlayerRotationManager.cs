using UnityEngine;

public class PlayerRotationManager : MonoBehaviour
{
    public PlayerRotationBaseState currentState;

    public Airborne airborne;
    public Grounded grounded;

    public PlayerRotationTypeBaseState typeState;

    public Keyboard keyboard;
    public MouseDelta mouse;

    public Transform player;

    private Vector3 angle = Vector3.zero;
    private Vector2 mouseDelta = Vector2.zero;

    private PlayerInput inputs;

    public float mouseHorizontalSensitivity = 300f;
    public float mouseVerticalSensitivity = 300f;

    public float HorizontalLerpSpeed = 50f;

    public float levelingIncline = 0f;

    [Header("Grounded Axes")]
    public InputAxes GroundedAxes = DefaultInputAxes;

    [Header("Airborne Axes")]
    public InputAxes AirborneAxes = DefaultInputAxes;

    public LayerMask layer;

    public bool interact;


    private void Start()
    {
        inputs = PlayerInput.Instance;

        airborne = new Airborne();
        grounded = new Grounded();

        currentState = grounded;

        keyboard = new Keyboard();
        mouse = new MouseDelta();

        typeState = mouse;

    }

    public void UpdateRotation()
    {
        currentState.Rotate(this);
    }

    public void SwitchState(PlayerRotationBaseState player)
    {
        currentState = player;
    }

    #region Player inputs
    public float GetTurnLeft()
    {
        return 1;
    }

    public float GetTurnRight()
    {
        return 1;
    }

    public float GetRise()
    {
        return 1;
    }

    public float GetPlunge()
    {
        return 1;
    }
    
    /**
    public float GetTurnLeft()
    {
        return inputs.inputs.TurnLeft.ReadValue<float>();
    }

    public float GetTurnRight()
    {
        return inputs.inputs.TurnRight.ReadValue<float>();
    }

    public float GetRise()
    {
        return inputs.inputs.Rise.ReadValue<float>();
    }

    public float GetPlunge()
    {
        return inputs.inputs.Plunge.ReadValue<float>();
    }
    */
    #endregion


    #region Rotation functions
    public void PlayerGroundedRotation()
    {
        float playerAngle = GetPlayerAngle() - 90;

        mouseDelta = typeState.GetMouseDelta(this);

        angle.x = mouseDelta.x * mouseHorizontalSensitivity * Time.deltaTime;
        angle.y = -mouseDelta.y * mouseVerticalSensitivity * Time.deltaTime;

        angle.y = AngleInRange(playerAngle, angle.y, GroundedAxes.VerticalAxis.Range);


        transform.Rotate(Vector3.up, angle.x, Space.World);
        transform.Rotate(Vector3.right, angle.y);
    }

    public void PlayerAirborneRotation()
    {
        mouseDelta = typeState.GetMouseDelta(this);

        if (mouseDelta.x != 0)
        {
            angle.x = mouseDelta.x * mouseHorizontalSensitivity * Time.deltaTime;
        }
        angle.y = -mouseDelta.y * mouseVerticalSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up, angle.x, Space.World);
        transform.Rotate(Vector3.right, angle.y);

        angle.x = Mathf.Lerp(angle.x, 0, HorizontalLerpSpeed * Time.deltaTime);
    }

    public void PlayerParallelRotation(Quaternion initialRotation, Quaternion finalRotation, float t)
    {
        transform.rotation = Quaternion.Slerp(finalRotation, initialRotation, t);
    }

    public void LevelPlayer()
    {

    }


    //Grounded mouse delta
    public Vector2 GetMouseDelta()
    {
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        
        return mouseDelta;
    }

    public float GetPlayerAngle()
    {
        return Vector3.Angle(transform.forward, Vector3.up);
    }

    public float AngleInRange(float angle, float value, Vector2 range)
    {
        if (value == 0)
        {
            return value;
        }

        float newAngle = angle + value;

        if (newAngle > range.y)
        {
            value -= newAngle - range.y;
        }
        else if (newAngle < range.x)
        {
            value += range.x - newAngle;
        }
        return value;
    }
    #endregion

    #region Player rotation type (keyboard/mouse) when flying
    public abstract class PlayerRotationTypeBaseState
    {
        public abstract Vector2 GetMouseDelta(PlayerRotationManager player);
    }

    public class Keyboard : PlayerRotationTypeBaseState
    {
        public override Vector2 GetMouseDelta(PlayerRotationManager player)
        {
            return new Vector2((player.GetTurnRight() - player.GetTurnLeft()) / 3, (player.GetRise() - player.GetPlunge()) / 5);
        }
    }

    public class MouseDelta : PlayerRotationTypeBaseState
    {
        public override Vector2 GetMouseDelta(PlayerRotationManager player)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            return mouseDelta;
        }
    }
    #endregion

    #region Player state rotation (grounded/airborne)
    public abstract class PlayerRotationBaseState
    {
        public abstract void Init(PlayerRotationManager player);
        public abstract void Rotate(PlayerRotationManager player);
    }

    public class Airborne : PlayerRotationBaseState
    {
        public override void Init(PlayerRotationManager player)
        {
            throw new System.NotImplementedException();
        }

        public override void Rotate(PlayerRotationManager player)
        {
            player.PlayerAirborneRotation();
        }
    }

    public class Grounded : PlayerRotationBaseState
    {
        public override void Init(PlayerRotationManager player)
        {
            throw new System.NotImplementedException();
        }

        public override void Rotate(PlayerRotationManager player)
        {
            player.PlayerGroundedRotation();
        }
    }
    #endregion

    #region Inspector structs
    [System.Serializable]
    public struct InputAxes
    {
        public InputAxis HorizontalAxis;
        public InputAxis VerticalAxis;

        public InputAxes(InputAxis _default)
        {
            HorizontalAxis = _default;
            VerticalAxis = _default;
        }
    }

    [System.Serializable]
    public struct InputAxis
    {
        public Vector2 Range;

        public InputAxis(Vector2 range)
        {
            Range = range;
        }
    }

    static InputAxis DefaultInputAxis => new InputAxis(new Vector2(0f, 0f));
    static InputAxes DefaultInputAxes => new InputAxes(DefaultInputAxis);
    #endregion
}