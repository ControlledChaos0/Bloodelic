using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableRoutine : BehaviorRoutine
{
    public override BehaviorState StartState {
        get => new MoveGridSelect();
    }

    /**
        Temporaries
    **/
    private int _tempMovement;
    public int TempMovement {
        get => _tempMovement;
        set => _tempMovement = value;
    }

    public override void GetTemporaries()
    {
        TempMovement = (behavior as Moveable).Movement;
    }
    public override void SetTemporaries()
    {
        (behavior as Moveable).Movement = TempMovement;
    }
}
