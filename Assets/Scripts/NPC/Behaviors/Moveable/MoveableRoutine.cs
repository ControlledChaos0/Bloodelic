using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableRoutine : BehaviorRoutine
{
    public override BehaviorState StartState {
        get => new MoveGridSelect();
    }
}
