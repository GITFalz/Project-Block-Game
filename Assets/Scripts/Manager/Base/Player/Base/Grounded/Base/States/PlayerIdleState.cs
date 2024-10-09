using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerIdleState : PlayerBaseState
{
    public override string stateName { get; } = "idle state";

    public override void EnterState(PlayerStateMachine state)
    {
        Debug.Log("Player entered idle state");

        state.afterJumpState = this;
        state.jumpVelocity = 0;

        state.manager.SetDrag(3);
        state.dashDirection = state.playerY.forward;
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        if (state.moveSwitch.CanSwitch())
        {
            Debug.Log("Switched to " + state.moveSwitch.currentState.stateName);
        }

        if (state.moveInput() != Vector2.zero)
        {
            state.manager.SetDrag(1);
            state.SwitchState(state.moveSwitch.currentState);
            return;
        }

        if (state.dashSwitch.CanSwitch())
        {
            state.manager.SetDrag(1);
            state.SwitchState(state.dashState);
            return;
        }

        if (state.jumpInput())
        {
            state.manager.SetDrag(1);
            state.SwitchState(state.jumpState);
            return;
        }

        if (state.isFalling(1.1f))
        {
            state.SwitchState(state.fallingState);
            return;
        }
    }

    public override void Physics(PlayerStateMachine state)
    {
        //state.floatingCapsule(25);

        state.dir = Vector3.Lerp(state.dir, Vector3.zero, .1f);

        state.SetVelocity(state.dir);
    }

    public override void ExitState(PlayerStateMachine state)
    {

    }
}
