using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KillableState : BehaviorState {
    private Killable _killable;
    private KillableRoutine _killableRoutine;

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
        _killable = behaviorRoutine.Behavior as Killable;
        _killableRoutine = behaviorRoutine as KillableRoutine;
        _killableRoutine.TempIsDead = true;
        TurnSystem.Instance.RemoveEntity(_killable.Human);
        _killable.ExecuteBehavior();
    }
    public void Deactivate() {
        
    }
}