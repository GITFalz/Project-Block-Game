using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSprintState : PlayerBaseState
{
    public Vector2 input;

    public override void EnterState(PlayerStateMachine state)
    {
        Debug.Log("Player entered sprint state");

        state.afterJumpState = this;
        state.jumpVelocity = state.playerData.GetSprintSpeed();
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        input = state.moveInput();

        if (state.moveSwitch.CanSwitch())
        {
            Debug.Log("Switched to " + state.moveSwitch.currentState.stateName);
            state.SwitchState(state.moveSwitch.currentState);
            return;
        }

        if (state.dashSwitch.CanSwitch())
        {
            state.dashDirection = (state.playerY.forward * input.y + state.playerY.right * input.x).normalized;
            state.SwitchState(state.dashState);
            return;
        }

        if (input == Vector2.zero)
        {
            state.SwitchState(state.idleState);
            return;
        }

        if (state.jumpSwitch.CanSwitch())
        {
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

        state.dir = (state.playerY.forward * input.y + state.playerY.right * input.x).normalized;
        state.flatMove(state.dir, state.playerData.GetSprintSpeed());
    }

    public override void ExitState(PlayerStateMachine state)
    {

    }
}
