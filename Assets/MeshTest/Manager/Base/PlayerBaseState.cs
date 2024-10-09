public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateMachine state);
    public abstract void UpdateState(PlayerStateMachine state);
    public abstract void ExitState(PlayerStateMachine state);
}