using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : BaseState
{
    public PlayerBaseState currentState;
    public PlayerBaseState oldState;
    public PlayerBaseState afterJumpState;
    
    public Rigidbody playerRigidbody;
    
    public PlayerIdleState idleState = new();
    public PlayerWalkState walkState = new();
    public PlayerRunState runState = new();
    public PlayerSprintState sprintState = new();
    public PlayerJumpState jumpState = new();
    public PlayerFallingState fallingState = new();
    public PlayerDashState dashState = new();
    
    public PlayerAdminState adminState = new();
    
    public UtilityManager manager;
    
    public RigidbodyManager rigidbodyManager;

    public Transform playerY;
    
    public Action<Vector3, float> freeMove;
    public Action<Vector3, float> flatMove;
    public Action<float> floatingCapsule;
    public Action<float> gravity;
    public Func<float, bool> isGrounded;
    public Func<float, bool> isFalling;
    public Action<Vector3> SetVelocity;

    //Input Functions
    public Func<Vector2> moveInput;
    public Func<bool> jumpInput;
    public Func<bool> controlInput;
    public Func<bool> rightClickInput;
    public Func<bool> shiftInput;
    
    public Func<bool> c;
    public Func<bool> e;
    public Func<bool> d;
    public Func<bool> p;

    //Player Data
    public PlayerData playerData;
    public Transform player;

    //Switches
    public Switch jumpSwitch;
    public Switch dashSwitch;
    
    public Switch pSwitch;
    public Switch eSwitch;
    public Switch dSwitch;
    
    public StateSwitch<PlayerBaseState> moveSwitch;

    public float jumpVelocity = 0;
    public Vector3 dashDirection = Vector3.zero;
    public Vector3 dir = Vector3.zero;

    private Vector3 velocity = Vector3.zero;

    public World worldSript;
    
    
    public List<Transform> points; 
    public List<float> times;
    public float smoothing = 0.5f;
    private float currentTime = 0f;
    private int currentSegment = 0;  
    
    private Vector3 playerPosition;
    
    public override void InitState(StateMachine state)
    {
        manager = UtilityManager.Instance;

        rigidbodyManager = state.rigidbodyManager;

        freeMove = rigidbodyManager.FreeMove;
        flatMove = rigidbodyManager.FlatMove;
        floatingCapsule = rigidbodyManager.FloatingCapsule;
        gravity = rigidbodyManager.Gravity;
        isGrounded = rigidbodyManager.isGrounded;
        isFalling = rigidbodyManager.isFalling;
        SetVelocity = rigidbodyManager.Set;

        moveInput = PlayerInput.Instance.MoveInput;
        jumpInput = PlayerInput.Instance.JumpInput;
        controlInput = PlayerInput.Instance.ControlInput;
        rightClickInput = PlayerInput.Instance.RightClickInput;
        shiftInput = PlayerInput.Instance.ShiftInput;
        
        c = PlayerInput.Instance.CInput;
        e = PlayerInput.Instance.EInput;
        d = PlayerInput.Instance.DInput;
        p = PlayerInput.Instance.PInput;
        
        pSwitch = new Switch(p);
        eSwitch = new Switch(e);
        dSwitch = new Switch(d);

        jumpSwitch = new Switch(jumpInput);
        dashSwitch = new Switch(rightClickInput);
        moveSwitch = new StateSwitch<PlayerBaseState>(controlInput, runState, walkState);

        playerData = PlayerData.instance;

        playerY = state.playerY;
        player = state.player;

        worldSript = state.worldScript;

        dashDirection = playerY.forward;

        afterJumpState = idleState;
        currentState = adminState;
        
        playerRigidbody = player.GetComponent<Rigidbody>();
        
        points = state.points;
        times = state.times;
    }

    public override void EnterState(StateMachine state)
    {
        Debug.Log("player is in playmode");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentState.EnterState(this);       
    }

    public override void UpdateState(StateMachine state)
    {
        if (controlInput() && c() && eSwitch.CanSwitch())
            state.doCinematic = true;
        if (controlInput() && c() && dSwitch.CanSwitch())
            state.doCinematic = false;
        if (controlInput() && c() && pSwitch.CanSwitch())
            state.CreatePoint();
        
        if (state.escapeSwitch.CanSwitch())
        {
            state.SwitchState(state.menuState);
            return;
        }

        state.playerRotationManager.UpdateRotation();
        currentState.UpdateState(this);
        
        HandleVelocity();
    }

    public override void PhysicsState(StateMachine state)
    {
        currentState.Physics(this);

        if (state.doCinematic)
        {
            if (currentSegment < times.Count)
            {
                // Update the time for the current segment
                currentTime += Time.fixedDeltaTime;
                float segmentTime = times[currentSegment];

                // If we are still within the current segment
                if (currentTime < segmentTime)
                {
                    // Calculate the t value (normalized time in the current segment)
                    float t = currentTime / segmentTime;

                    // Smooth interpolation based on the smoothing factor
                    Vector3 position = CatmullRomInterpolation(
                        GetPoint(currentSegment - 1), // Previous point
                        GetPoint(currentSegment), // Current point
                        GetPoint(currentSegment + 1), // Next point
                        GetPoint(currentSegment + 2), // After next point
                        t, // Interpolation value
                        smoothing // Smoothing factor
                    );

                    // Use Rigidbody.MovePosition to move smoothly
                    playerRigidbody.MovePosition(position);
                }
                else
                {
                    // Move to the next segment
                    currentSegment++;
                    currentTime = 0f;
                }
            }
        }
    }

    public override void ExitState(StateMachine state)
    {
        
    }

    public void SwitchState(PlayerBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void Gravity()
    {
        velocity.y -= 2f * Time.deltaTime;
        
        Debug.Log("1: " + velocity);

        Block[] blocks = null;

        for (int i = 0; i < 4; i++)
        {
            Vector3 futurePosition = player.position + velocity + playerHitBox[i];
        
            Vector3Int chunkPosition = Mathp.ChunkPos(futurePosition);
            
            Debug.Log($"{i} {chunkPosition} {futurePosition}");
            
            if (worldSript.worldData.activeChunkData.ContainsKey(chunkPosition))
            {
                blocks = worldSript.worldData.activeChunkData[chunkPosition].blocks;

                if (blocks[Mathp.PosIndex(futurePosition)] != null)
                {
                    Debug.Log($"{i} {Mathp.PosIndex(futurePosition)}");
                    velocity.y = 0;
                }
            } 
        }
        
        Debug.Log("2: " + velocity);
    }

    public void Move(Vector3 direction, float maxSpeed)
    {
        playerRigidbody.MovePosition(playerRigidbody.position + maxSpeed * Time.deltaTime * direction);
    }

    public void HandleVelocity()
    {
        player.position += velocity;
    }

    public static Vector3[] playerHitBox = new Vector3[]
    {
        new Vector3(-0.4f,-0.9f,-0.4f),
        new Vector3( 0.4f,-0.9f,-0.4f),
        new Vector3( 0.4f,-0.9f, 0.4f),
        new Vector3(-0.4f,-0.9f, 0.4f),
    };
    
    
    
    private Vector3 GetPoint(int index)
    {
        if (index < 0) return points[0].position; // Use the first point for out-of-bounds indices
        if (index >= points.Count) return points[points.Count - 1].position; // Use the last point
        return points[index].position;
    }

    // Catmull-Rom Spline Interpolation
    private Vector3 CatmullRomInterpolation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, float smoothFactor)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        // Catmull-Rom formula
        Vector3 result = 0.5f *
                         ((2f * p1) +
                          (-p0 + p2) * t +
                          (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                          (-p0 + 3f * p1 - 3f * p2 + p3) * t3);

        // Blend between linear interpolation and Catmull-Rom smoothing
        return Vector3.Lerp(Vector3.Lerp(p1, p2, t), result, smoothFactor);
    }
    
    // Get the rotation for a given index
    private Quaternion GetRotation(int index)
    {
        if (index < 0) return points[0].rotation; // Use the first rotation for out-of-bounds indices
        if (index >= points.Count) return points[points.Count - 1].rotation; // Use the last rotation
        return points[index].rotation;
    }

// Quaternion Catmull-Rom Interpolation for rotations
    private Quaternion CatmullRomRotation(Quaternion r0, Quaternion r1, Quaternion r2, Quaternion r3, float t, float smoothFactor)
    {
        // Interpolate using spherical linear interpolation (SLerp)
        Quaternion slerp1 = Quaternion.Slerp(r0, r1, t);
        Quaternion slerp2 = Quaternion.Slerp(r1, r2, t);
        Quaternion slerp3 = Quaternion.Slerp(r2, r3, t);

        // Perform a weighted blend between linear and Catmull-Rom
        Quaternion result = Quaternion.Slerp(slerp2, Quaternion.Slerp(slerp1, slerp3, t), smoothFactor);
        return result;
    }
}
