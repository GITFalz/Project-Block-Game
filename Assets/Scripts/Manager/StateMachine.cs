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
    public MainMenu mainMenu;

    //Player Data
    public PlayerData playerData;

    public Transform playerY;
    public Transform player;

    public World worldScript;

    public Func<bool> escapeInput;

    //Switches
    public Switch escapeSwitch;
    
    public List<Transform> points; 
    public List<float> times;
    public List<Vector3> angles;
    
    public bool doCinematic = false;
    
    public GameObject pointPrefab;

    private void Start()
    {
        manager = UtilityManager.Instance;
        playerData = PlayerData.instance;

        escapeSwitch = new Switch(PlayerInput.Instance.EscapeInput);
        
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

    public void CreatePoint()
    {
        Vector3 position = player.GetComponent<Rigidbody>().position;
        GameObject instance = Instantiate(pointPrefab, position, Quaternion.identity);
        points.Add(instance.transform);
        times.Add(5);
    }
    
    public void CreateAngle()
    {
        angles.Add(player.eulerAngles);
    }
}
