using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can't derive from StateMachine bc it's a Singleton
public class SelectStateMachine : Singleton<SelectStateMachine>
{
    public SelectState currentState;
    private SearchSelectState _searchState = new();
    private UISelectState _uiState;
    private Selectable _selectable;
    private WorldUI _currUI;
    private BehaviorController _currBehavCont;
    public SearchSelectState SearchState {
        get => _searchState;
    }
    public UISelectState UIState {
        get => _uiState;
        set => _uiState = value;
    }
    public Selectable Selectable {
        get => _selectable;
        set {
            _selectable = value;
        }
    }
    public WorldUI CurrUI {
        get => _currUI;
        set => _currUI = value;
    }
    public BehaviorController CurrBehavCont {
        get => _currBehavCont;
        set => _currBehavCont = value;
    }

    protected virtual void Awake() {
        InitializeSingleton();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _searchState = new();
        StartState(_searchState);
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
