using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class Holdable : Behavior
{
    /**
        Controlling Thing
    **/
    private Human _human;
    public Human Human {
        get => _human;
        set => _human = value;
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
        get => "Drain";
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
        return new KillableRoutine();
    }
    public override bool CheckValid()
    {
        Debug.Log($"Movement Remaining: {_movement}");
        return _movement > 0;
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
