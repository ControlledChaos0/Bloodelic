using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State currentState;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //currentState.StartState(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentState?.UpdateState();
    }

    
    public virtual void StartState(State state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState = state;
        currentState.StartState(this);
    }
    public virtual void ChangeState(State state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState?.ExitState();
        StartState(state);
    }
}
