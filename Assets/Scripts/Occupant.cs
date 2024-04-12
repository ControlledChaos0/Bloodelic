using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BehaviorController))]
public abstract class Occupant : MonoBehaviour
{
    [SerializeField]
    protected GridCell occupiedCell;
    [SerializeField]
    protected new Collider collider;
    protected GridCell prevOccupiedCell;
    protected BehaviorController behaviorController;

    public GridCell OccupiedCell {
        get => occupiedCell;
        set => occupiedCell = value;
    }
    public abstract bool BlockCells {
        get;
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        behaviorController = GetComponent<BehaviorController>();
        behaviorController.InitializeBehaviors();

        Vector3 vec = transform.rotation * Vector3.down;
        if (Physics.Raycast(collider.bounds.center, vec, out RaycastHit hit, Mathf.Infinity, 1 << 3))
        {
            Debug.Log("Hits!");
            occupiedCell = hit.transform.GetComponent<GridCell>();
            prevOccupiedCell = occupiedCell;
            //testCell = occupiedCell;
            // Set occupant for occupied cell
            occupiedCell.SetOccupant(this);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public void SetOccupation(GridCell cell)
    {
        if (cell == null) { return; }
        occupiedCell.Unoccupy();
        occupiedCell = cell;
        // Set occupant for occupied cell
        occupiedCell.SetOccupant(this);
    }
}
