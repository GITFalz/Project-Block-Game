using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState currentState;

    public PlayerStateMachine playerState = new();
    public MenuStateMachine menuState = new();
    
    public UtilityManager manager;
    
    public RigidbodyManager rigidbodyManager;
    public PlayerRotationManager playerRotationManager;

    public GameObject menu;

    //Player Data
    public PlayerData playerData;

    public Transform playerY;
    public Transform player;

    public World worldScript;

    public Func<bool> escapeInput;

    //Switches
    public Switch escapeSwitch;

    private void Start()
    {
        manager = UtilityManager.Instance;

        playerData = PlayerData.instance;

        escapeInput = InputManager.EscapeInput;

        escapeSwitch = new Switch(escapeInput);
        
        menuState.InitState(this);
        playerState.InitState(this);

        menuState = new MenuStateMachine();
        
        currentState = menuState;
        currentState.EnterState(this);
        
        
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.PhysicsState(this);
    }

    public void SwitchState(BaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }
}
