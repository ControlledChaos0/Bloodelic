using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SmallHoldableState : BehaviorState {
    private SmallHoldable _smallHoldable;
    private SmallHoldableRoutine _smallHoldableRoutine;
    private GridCell[] _neighbors;
    private bool _startHeld;

    public override void EnterState()
    {
        Activate();
    }
    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    {
        Deactivate();
    }

    public void Activate() {
        _smallHoldable = behaviorRoutine.Behavior as SmallHoldable;
        _smallHoldableRoutine = behaviorRoutine as SmallHoldableRoutine;
        _startHeld = _smallHoldableRoutine.TempIsHeld;
        if (_startHeld) {
            ActivatePutDown();
        } else {
            ActivatePickUp();
        }
    }
    private void ActivatePutDown() {
        _neighbors = _smallHoldable.HeldEntity.OccupiedCell.Neighbors;
        foreach (GridCell gridCell in _neighbors) {
            gridCell.ShowCell();
        }
    }
    private void ActivatePickUp() {
        _smallHoldableRoutine.TempIsHeld = true;
        _smallHoldableRoutine.TempHeldEntity = TurnSystem.Instance.ActiveEntity;
        _smallHoldable.ExecuteBehavior();
    }
    public void Deactivate() {
        if (_startHeld) {
            DeactivatePutDown();
        } else {
            DeactivatePickUp();
        }
    }
    private void DeactivatePutDown() {
        foreach (GridCell gridCell in _neighbors) {
            gridCell.HideCell();
        }
        _neighbors = null;
    }
    private void DeactivatePickUp() {

    }
}