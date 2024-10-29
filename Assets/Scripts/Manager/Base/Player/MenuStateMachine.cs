using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStateMachine : BaseState
{
    public override void InitState(StateMachine state)
    {
        return;
    }

    public override void EnterState(StateMachine state)
    {
        state.menu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void UpdateState(StateMachine state)
    {
        if (state.escapeSwitch.CanSwitch())
        {
            state.SwitchState(state.playerState);
            return;
        }
    }

    public override void PhysicsState(StateMachine state)
    {
        return;
    }

    public override void ExitState(StateMachine state)
    {
        state.menu.SetActive(false);
        state.mainMenu.CloseAll();
    }
}
