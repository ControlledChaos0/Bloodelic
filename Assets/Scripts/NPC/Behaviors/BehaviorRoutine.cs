using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorRoutine
{
    protected Behavior behavior;
    protected BehaviorState currentState;
    public Behavior Behavior {
        get => behavior;
        set => behavior = value;
    }
    //StartState must be implemented with the first state of the concrete BehaviorRoutine
    public abstract BehaviorState StartState {
        get;
    }

    public virtual void StartBehavior() {
        InputController.Instance.Cancel += CancelBehavior;
        SelectStateMachine.Instance.StartRoutine(this);
    }
    public virtual void EndBehavior() {
        InputController.Instance.Cancel -= CancelBehavior;
        SelectStateMachine.Instance.EndRoutine();
    }
    public virtual void ExecuteBehavior() {
        EndBehavior();
    }
    public virtual void CancelBehavior() {
        EndBehavior();
    }
    public virtual void Start(BehaviorState behaviorState) {
        currentState = behaviorState;
    }
}
