using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public abstract void InitState(StateMachine state);
    public abstract void EnterState(StateMachine state);
    public abstract void UpdateState(StateMachine state);
    public abstract void PhysicsState(StateMachine state);
    public abstract void ExitState(StateMachine state);
}
