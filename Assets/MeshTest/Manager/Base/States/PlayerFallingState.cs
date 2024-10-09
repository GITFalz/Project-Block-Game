public class PlayerFallingState : PlayerBaseState
{
    public override void EnterState(PlayerStateMachine state)
    {
        
    }

    public override void UpdateState(PlayerStateMachine state)
    {
        state.ApplyGravity();
    }

    public override void ExitState(PlayerStateMachine state)
    {
        
    }
}