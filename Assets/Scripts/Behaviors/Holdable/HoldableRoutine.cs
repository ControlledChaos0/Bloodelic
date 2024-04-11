using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HodlableRoutine : BehaviorRoutine
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
        Pathfinder.moveLimit = TempMovement;
    }
    public override void SetTemporaries()
    {
        Debug.Log("Bro are you running");
        (behavior as Moveable).Movement = TempMovement;
        Debug.Log($"Movement: {(behavior as Moveable).Movement}");
        Pathfinder.moveLimit = Mathf.Infinity;
    }
}
