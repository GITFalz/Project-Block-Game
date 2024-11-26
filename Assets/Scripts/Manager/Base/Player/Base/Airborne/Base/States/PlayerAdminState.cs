using UnityEngine;

public class PlayerAdminState : PlayerBaseState
{
    public Vector2 input;

    public float timer;

    public override void EnterState(PlayerStateMachine state)
    {
        Debug.Log("Player entered admin state");
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        
    }

    public override void Physics(PlayerStateMachine state)
    {
        input = state.moveInput();
        
        Vector3 direction = new Vector3(0, 0, 0);
        
        if (state.moveInput() != Vector2.zero)
        {
            direction += state.playerY.forward * input.y + state.playerY.right * input.x;
        }

        if (state.jumpInput())
        {
            direction += Vector3.up;
        }

        if (state.shiftInput())
        {
            direction += Vector3.down;
        }
        
        state.Move(direction, 50);
    }

    public override void ExitState(PlayerStateMachine state)
    {

    }
}