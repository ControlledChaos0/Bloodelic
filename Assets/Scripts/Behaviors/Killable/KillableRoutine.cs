using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableRoutine : BehaviorRoutine
{
    public override BehaviorState StartState {
        get => new KillableState();
    }

    /**
        Temporaries
    **/
    private bool _tempIsDead;
    public bool TempIsDead {
        get => _tempIsDead;
        set => _tempIsDead = value;
    }

    
    public override void GetTemporaries()
    {
        TempIsDead = (behavior as Killable).IsDead;
    }
    public override void SetTemporaries()
    {
        Debug.Log("Bro are you running");
        (behavior as Killable).IsDead = TempIsDead;
    }
}
