using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can't derive from StateMachine bc it's a Singleton
public class SelectStateMachine : Singleton<SelectStateMachine>
{
    private Selectable _selectable;
    public Selectable Selectable {
        get => _selectable;
        set => _selectable = value;
    }
    public SelectState currentState;
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

    
    public void StartState(SelectState state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState = state;
        currentState.StartState(Instance);
    }
    public void ChangeState(SelectState state) {
        ArgumentNullExceptionUse.ThrowIfNull(state); 

        currentState?.ExitState();
        StartState(state);
    }

    public void StartSelectable(Selectable selectable) {
        _selectable = selectable;
        StartState(new UISelectState());
    }

    public void ClearSelectable() {
        _selectable.Deactivate();
        _selectable = null;
        currentState?.ExitState();
        currentState = null;
    }
}
