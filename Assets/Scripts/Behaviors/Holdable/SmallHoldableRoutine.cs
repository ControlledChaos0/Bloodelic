using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallHoldableRoutine : BehaviorRoutine
{
    public override BehaviorState StartState {
        get => new SmallHoldableState();
    }

    /**
        Temporaries
    **/
    private bool _tempIsHeld;
    private Entity _tempHeldEntity;
    public bool TempIsHeld {
        get => _tempIsHeld;
        set => _tempIsHeld = value;
    }
    public Entity TempHeldEntity {
        get => _tempHeldEntity;
        set => _tempHeldEntity = value;
    }

    
    public override void GetTemporaries()
    {
        TempIsHeld = (behavior as SmallHoldable).IsHeld;
        TempHeldEntity = (behavior as SmallHoldable).HeldEntity;
    }
    public override void SetTemporaries()
    {
        Debug.Log("Bro are you running");
        (behavior as SmallHoldable).IsHeld = TempIsHeld;
        (behavior as SmallHoldable).HeldEntity = TempHeldEntity;
    }
}
