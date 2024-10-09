using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerDashState : PlayerBaseState
{
    public Vector2 input;

    public float timer;

    public override void EnterState(PlayerStateMachine state)
    {
        Debug.Log("Player entered dash state");

        timer = .2f;

        state.jumpVelocity = state.playerData.GetSprintSpeed();

        Vector3 direction = state.dashDirection;
        if (state.oldState.stateName.Equals("idle state"))
        {
            state.flatMove(direction, -state.playerData.GetDashSpeed());
        }
        else
        {
            state.flatMove(direction, state.playerData.GetDashSpeed());
        }
        
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        input = state.moveInput();

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (state.rightClickInput())
            {
                state.SwitchState(state.sprintState);
                return;
            }

            if (state.moveInput() != Vector2.zero)
            {
                state.SwitchState(state.moveSwitch.currentState);
                return;
            }

            if (state.jumpSwitch.CanSwitch())
            {
                state.SwitchState(state.jumpState);
                return;
            }

            if (state.moveInput() == Vector2.zero)
            {
                state.SwitchState(state.idleState);
                return;
            }

            if (state.isFalling(1.1f))
            {
                state.SwitchState(state.fallingState);
                return;
            }
        }
    }

    public override void Physics(PlayerStateMachine state)
    {
        //state.floatingCapsule(25);
    }

    public override void ExitState(PlayerStateMachine state)
    {

    }
}
