using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;
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

    protected virtual void ChangeState(State state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState?.ExitState();
        currentState = state;
        currentState.StartState(this);
    }
}
