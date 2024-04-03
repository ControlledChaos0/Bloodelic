using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class Moveable : Behavior
{
    /**
        Controlling Thing
    **/
    private Monster _monster;
    public Monster Monster {
        get => _monster;
        set => _monster = value;
    }

    /**
        Behavior Specific Variables
    **/
    private int _movement;

    public int Movement {
        get => _movement;
        set => _movement = value;
    }

    /**
        Identifiers
    **/
    public override string Name {
        get => "Move";
    }
    public override bool NPCInteract {
        get => false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override BehaviorRoutine CreateRoutine()
    {
        return new MoveableRoutine();
    }
    public override bool CheckValid()
    {
        Debug.Log($"Movement Remaining: {_movement}");
        return _movement > 0;
    }
    public override void ResetBehaviorSpecifics()
    {
        _movement = Monster.Movement;
    }
    public override IEnumerator ExecuteBehaviorCoroutine()
    {
        Debug.Log("Executing Moveable Routine");
        //yield return null;
        yield return Monster.IterateThroughSpline();
        SelectStateMachine.Instance.EndRoutine();
    }
}
