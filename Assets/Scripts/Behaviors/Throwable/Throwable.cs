using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class Throwable : Behavior
{
    /**
        Controlling Thing
    **/
    private SmallHoldableObject _object;
    public SmallHoldableObject Object {
        get => _object;
        set => _object = value;
    }

    ParabolaObject _parabola;

    public ParabolaObject ParabolaObject
    {
        get => _parabola;
        set => _parabola = value;
    }

    public GameObject thrownObject;
    
    /**
        Behavior Specific Variables
    **/
    [SerializeField]
    private int _throwRange = 5;

    public int Range {
        get => _throwRange;
        set => _throwRange = value;
    }

    public GridCell startCell;
    public GridCell targetCell;

    [SerializeField] float noiseRadius = 2f;
    
    /**
        Identifiers
    **/
    public override string Name {
        get => "Throw";
    }
    public override bool NPCInteract {
        get => false;
    }
    public override bool Resetable {
        get => true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override BehaviorRoutine CreateRoutine()
    {
        return new ThrowableRoutine();
    }
    public override bool CheckValid()
    {
        Debug.Log($"Range: {Range}");
        foreach (GridCell gridCell in Object.OccupiedCell.Neighbors) {
            if (!gridCell.IsOccupied()) {
                continue;
            }
            if (gridCell.Occupant.Equals(TurnSystem.Instance.ActiveEntity)) {
                return true;
            }
        }
        return false;
    }
    public override void ResetBehaviorSpecifics()
    {
    }
    public override void GetControllingComponent()
    {
        _object = GetComponent<SmallHoldableObject>();
        _parabola = FindObjectOfType<ParabolaObject>();
    }
    public override IEnumerator ExecuteBehaviorCoroutine()
    {
        Outline outline = Object.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
        
        // Set up parabola
        _parabola.startTransform = startCell.transform;
        _parabola.endTransform = targetCell.transform;
        _parabola.thrownObject = thrownObject;
        
        Debug.Log("Executing Throwable Routine");
        yield return _parabola.MoveAlongCurve();
        _object.transform.position = targetCell.transform.position;
        SelectStateMachine.Instance.EndRoutine(true);
        _object.Selectable.enabled = false;
        
        // Put any npc within noise range into distressed
        List<Human> humans = FindObjectsOfType<Human>().ToList();
        foreach (Human npc in humans)
        {
            if (Vector3.Distance(npc.transform.position, targetCell.transform.position) <= noiseRadius)
            {
                AIBrain ai = npc.GetComponent<AIBrain>();
                ai.ChangeState(AIState.Distressed);
            }
        }
        Destroy(_object.gameObject);
    }
}
