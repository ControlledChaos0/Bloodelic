using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : IGameState
{
    protected StateMachine stateMachine;

    public void StartState(StateMachine sm) {
        stateMachine = sm;
    }
    public abstract void EnterState(StateMachine sm);
    public abstract void UpdateState(StateMachine sm);
    public abstract void ExitState(StateMachine sm);
}
