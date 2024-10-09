using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerJumpState : PlayerBaseState
{
    public Vector2 input;

    public override void EnterState(PlayerStateMachine state)
    {
        Debug.Log("Player entered jump state");

        Vector3 direction = Vector3.up * state.playerData.GetJumpHeight();
        state.freeMove(direction, 1);
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        input = state.moveInput();

        if (state.isFalling(0.9f))
        {
            state.SwitchState(state.fallingState);
            return;
        }      
    }

    public override void Physics(PlayerStateMachine state)
    {
        state.gravity(5);

        Vector3 direction = (state.playerY.forward * input.y + state.playerY.right * input.x).normalized;

        if (direction.Equals(Vector3.zero))
        {
            state.dir = Vector3.Lerp(state.dir, Vector3.zero, .1f);
        }
        else
        {
            state.dir = direction;
        }
            
        state.flatMove(state.dir, state.playerData.GetWalkSpeed());
    }

    public override void ExitState(PlayerStateMachine state)
    {

    }
}
