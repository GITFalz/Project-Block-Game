using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    public Vector2 input;

    public override void EnterState(PlayerStateMachine state)
    {
        Debug.Log("Player entered falling state");
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        input = state.moveInput();
        
        state.Gravity();

        if (state.isGrounded(.9f))
        {
            state.SwitchState(state.idleState);
            return;
        }
    }

    public override void Physics(PlayerStateMachine state)
    {
        /**
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

        state.gravity(5);
        */
    }

    public override void ExitState(PlayerStateMachine state)
    {

    }
}
