using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectState : IGameState
{
    protected SelectStateMachine stateMachine;

    public virtual void StartState(SelectStateMachine sm) {
        stateMachine = sm;
        EnterState();
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
