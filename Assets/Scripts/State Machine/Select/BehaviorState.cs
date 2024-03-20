using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorState : IGameState
{
    protected BehaviorRoutine behaviorRoutine;
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
