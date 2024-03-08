using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can't derive from StateMachine bc it's a Singleton
public class SelectStateMachine : Singleton<SelectStateMachine>
{
    private static Selectable _selectable;
    public static Selectable Selectable {
        get => _selectable;
        set => _selectable = value;
    }
    public static SelectState currentState;
    protected virtual void Awake() {
        InitializeSingleton();
    }
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

    
    public static void StartState(SelectState state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState = state;
        currentState.StartState(Instance);
    }
    public static void ChangeState(SelectState state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState?.ExitState();
        StartState(state);
    }

    public static void ClearSelectable() {
        _selectable.Deactivate();
        _selectable = null;
        currentState?.ExitState();
        currentState = null;
    }
}
