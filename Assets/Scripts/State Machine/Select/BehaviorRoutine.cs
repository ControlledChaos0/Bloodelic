using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorRoutine
{
    protected BehaviorState currentState;
    public BehaviorRoutine() {
        StartBehavior();
    }

    public abstract void StartBehavior();
    public abstract void EndBehavior();
}
