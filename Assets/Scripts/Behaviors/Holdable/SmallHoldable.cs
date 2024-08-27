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
    [SerializeField]
    private bool _isHeld;
    [SerializeField]
    private float _pickUpSpeed = 10f;

    private bool _track;
    private Entity _heldEntity;
    private Vector3 _objPos;
    private Vector3 _objDir;

    public bool IsHeld {
        get => _isHeld;
        set => _isHeld = value;
    }
    public Entity HeldEntity {
        get => _heldEntity;
        set => _heldEntity = value;
    }

    /**
        Identifiers
    **/
    public override string Name {
        get {
            if (_isHeld) {
                return "Put Down";
            }
            return "Pick Up";
        }
    }
    public override bool NPCInteract {
        get => true;
    }
    public override bool Resetable {
        get => false;
    }

    protected void Start() {
        _track = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (HeldEntity != null && _track) {
            transform.position = HeldEntity.transform.position;
        }
    }

    public override BehaviorRoutine CreateRoutine()
    {
        return new SmallHoldableRoutine();
    }
    public override bool CheckValid()
    {
        //return _movement > 0;
        if (_isHeld) {
            foreach (GridCell gridCell in _heldEntity.OccupiedCell.Neighbors) {
                if (!gridCell.IsOccupied()) {
                    return true;
                }
            }
            return false;
        } else {
            foreach (GridCell gridCell in Object.OccupiedCell.Neighbors) {
                if (gridCell.BlockOccupant == null) {
                    continue;
                }
                if (gridCell.BlockOccupant.Equals(TurnSystem.Instance.ActiveEntity)) {
                    return true;
                }
            }
            return false;
        }
    }
    public override void ResetBehaviorSpecifics()
    {
        return;
    }
    public override void GetControllingComponent()
    {
        _object = GetComponent<SmallHoldableObject>();
    }
    public override void ExecuteBehavior() {

        behaviorRoutine.ExecuteBehaviorRoutine();
        _objPos = _object.transform.position;
        _objDir = (_heldEntity.transform.position - _objPos).normalized;
        StartCoroutine(ExecuteBehaviorCoroutine());
    }
    public override IEnumerator ExecuteBehaviorCoroutine()
    {
        Debug.Log("Executing Small Holdable Routine");
        if (!_isHeld) {
            _track = false;
            _object.OccupiedCell.Unoccupy(_object);
            yield return PickUpCoroutine();
            _isHeld = true;
            _track = true;
            Object.Selectable.ModelObject.SetActive(false);
            SelectStateMachine.Instance.EndRoutine(true);
        } else {
            _track = false;
            Object.Selectable.ModelObject.SetActive(true);
            yield return PutDownCoroutine();
            _track = true;
            _isHeld = false;
            SelectStateMachine.Instance.EndRoutine(true);
        }
    }

    private IEnumerator PickUpCoroutine() {
        while (Vector3.Dot(_objDir, (_heldEntity.transform.position - _object.transform.position).normalized) > 0) {
            _object.transform.position += _objDir * _pickUpSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        (_heldEntity as Monster).Inventory.addItemToInventory(_object);
    }
    private IEnumerator PutDownCoroutine() {
        //if () {
            yield return new WaitForFixedUpdate();
        //}
        _object.enabled = true;
    }
}
