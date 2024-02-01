using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
    [SerializeField] private TurnDisplay turnDisplay;
    public Entity[] turnOrder;
    public Entity activeEntity;
    private int _activeEntityIdx;

    void Awake ()
    {
        InitializeSingleton();
        _activeEntityIdx = 0;
        activeEntity = turnOrder[0];
    }
    
    void Start() {
        turnDisplay.UpdateDisplays(turnOrder, _activeEntityIdx);
        UpdateInputController();
        ActiveEntityAction();
    }

    public void SwitchTurn() {
        _activeEntityIdx = (_activeEntityIdx + 1) % turnOrder.Length;
        activeEntity = turnOrder[_activeEntityIdx];
        turnDisplay.UpdateDisplays(turnOrder, _activeEntityIdx);
        UpdateInputController();
        ActiveEntityAction();
    }

    public void HandlePlayerTurn() {
        if (activeEntity is Monster && !activeEntity.IsMoving) {
            SwitchTurn();
        }
    }

    private void ActiveEntityAction() {
        if (activeEntity is Human) {
            ((Human)activeEntity).PerformAction();
        }
    }

    private void UpdateInputController() {
        if (activeEntity is Monster) {
            InputController.Instance.enabled = true;
        } else {
            InputController.Instance.enabled = false;
        }
    }


}
