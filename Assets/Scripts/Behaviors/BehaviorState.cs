using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public abstract class BehaviorState : SelectState
{
    protected BehaviorRoutine behaviorRoutine;

    public override void StartState(SelectStateMachine sm) {
        behaviorRoutine = sm.CurrBehavRoutine;
        behaviorRoutine.Start(this);
        base.StartState(sm);
    }
}
