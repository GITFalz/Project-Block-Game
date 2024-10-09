using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Singleton instance
    public static PlayerInput instance;

    // Reference to the input actions
    public PlayerController.PlayerInputsActions playerInputs;

    private void Awake()
    {
        // Set up Singleton pattern to ensure a single instance
        if (instance == null)
        {
            instance = this;
        }

        // Initialize the input actions
        playerInputs = new PlayerController.PlayerInputsActions();
    }

    private void OnEnable()
    {
        // Enable the input actions
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        // Disable the input actions
        playerInputs.Disable();
    }

    public Vector2 GetMoveInputs()
    {
        return playerInputs.Move.ReadValue<Vector2>();
    }

    public bool GetTurnLeft()
    {
        return playerInputs.TurnLeft.ReadValue<float>() > .5f;
    }
    
    public bool GetTurnRight()
    {
        return playerInputs.TurnRight.ReadValue<float>() > .5f;
    }

    public bool GetUp()
    {
        return playerInputs.GoUp.ReadValue<float>() > .5f;
    }

    public bool GetDown()
    {
        return playerInputs.GoDown.ReadValue<float>() > .5f;
    }
}
