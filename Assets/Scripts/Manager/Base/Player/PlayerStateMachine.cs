using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : BaseState
{
    public PlayerBaseState currentState;
    public PlayerBaseState oldState;
    public PlayerBaseState afterJumpState;
    
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

    //Player Data
    public PlayerData playerData;
    public Transform player;

    //Switches
    public Switch jumpSwitch;
    public Switch dashSwitch;
    public StateSwitch<PlayerBaseState> moveSwitch;

    public float jumpVelocity = 0;
    public Vector3 dashDirection = Vector3.zero;
    public Vector3 dir = Vector3.zero;

    private Vector3 velocity = Vector3.zero;

    public World worldSript;
    
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
        player.position += maxSpeed * Time.deltaTime * direction;
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
}
