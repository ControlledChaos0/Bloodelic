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

    public virtual void StartBehaviorRoutine() {
        GetTemporaries();
        InputController.Instance.Cancel += CancelBehaviorRoutine;
        SelectStateMachine.Instance.StartRoutine(this);
    }
    public virtual void CancelBehaviorRoutine() {
        InputController.Instance.Cancel -= CancelBehaviorRoutine;
        SelectStateMachine.Instance.EndRoutine();
    }
    public virtual void ExecuteBehaviorRoutine() {
        InputController.Instance.Cancel -= CancelBehaviorRoutine;
        SetTemporaries();
    }
    public virtual void Start(BehaviorState behaviorState) {
        currentState = behaviorState;
    }

    public abstract void GetTemporaries();
    public abstract void SetTemporaries();
}
