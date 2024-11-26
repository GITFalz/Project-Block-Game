using UnityEngine;

public class CinematicStateMachine : BaseState
{
    public bool inMenu = true;
    
    public override void InitState(StateMachine state)
    {
        return;
    }

    public override void EnterState(StateMachine state)
    {
        state.cinematic.SetActive(true);
        state.cinematicMovementManager.isPlaying = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void UpdateState(StateMachine state)
    {
        state.cinematicSwitch.CanSwitch();

        if (state.cinematicSwitch.currentState)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            state.playerState.PartialUpdate(state);
        }
        
        if (state.escapeSwitch.CanSwitch())
        {
            state.SwitchState(state.playerState);
            return;
        }
    }

    public override void PhysicsState(StateMachine state)
    {
        if (state.cinematicSwitch.currentState)
            return;
        
        state.playerState.PhysicsState(state);
    }

    public override void ExitState(StateMachine state)
    {
        state.cinematic.SetActive(false);
    }
}