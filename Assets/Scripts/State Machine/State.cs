using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : IGameState
{
    protected StateMachine stateMachine;

    public virtual void StartState(StateMachine sm) {
        stateMachine = sm;
        EnterState();
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
