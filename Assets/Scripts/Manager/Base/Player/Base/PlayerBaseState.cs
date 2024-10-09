using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    public virtual string stateName { get; } = "Default";
    public abstract void EnterState(PlayerStateMachine state);
    public abstract void UpdateState(PlayerStateMachine state);
    public abstract void Physics(PlayerStateMachine state);
    public abstract void ExitState(PlayerStateMachine state);
}
