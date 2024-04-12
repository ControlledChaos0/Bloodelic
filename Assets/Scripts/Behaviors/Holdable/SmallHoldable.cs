using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class SmallHoldable : Behavior
{
    /**
        Controlling Thing
    **/
    private SmallHoldableObject _object;
    public SmallHoldableObject Object {
        get => _object;
        set => _object = value;
    }

    /**
        Behavior Specific Variables
    **/
    private bool _isHeld;
    public bool IsHeld {
        get => _isHeld;
        set => _isHeld = value;
    }

    /**
        Identifiers
    **/
    public override string Name {
        get {
            if (IsHeld) {
                return "Put Down";
            }
            return "Pick Up";
        }
    }
    public override bool NPCInteract {
        get => false;
    }
    public override bool Resetable {
        get => false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override BehaviorRoutine CreateRoutine()
    {
        return new SmallHoldableRoutine();
    }
    public override bool CheckValid()
    {
        //return _movement > 0;
        return false;
    }
    public override void ResetBehaviorSpecifics()
    {
        //No resetting
    }
    public override IEnumerator ExecuteBehaviorCoroutine()
    {
        Debug.Log("Executing Moveable Routine");
        //yield return null;
        yield return null;
        SelectStateMachine.Instance.EndRoutine();
    }
}
