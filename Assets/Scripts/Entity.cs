using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS3009 // Base type is not CLS-compliant
public class Entity : MonoBehaviour
#pragma warning restore CS3009 // Base type is not CLS-compliant
{
    [SerializeField]
    protected new Collider collider;
    [SerializeField]
    protected GridCell occupiedCell;
    [SerializeField]
    protected float moveSpeed = 10f;
    [SerializeField]
    protected float rotateSpeed = 10f;

    protected Vector3 GroundPosition => transform.position + (transform.rotation * offset);
    protected Vector3 OffsetGridPos => occupiedCell.transform.position + (occupiedCell.transform.rotation * -offset);
    protected Vector3 OffsetPrevGridPos => prevOccupiedCell.transform.position + (prevOccupiedCell.transform.rotation * -offset);
    protected GridPath linkedPath;
    protected GridCell prevOccupiedCell;
    protected Vector3 offset;
    protected float height;
    protected float time = 0;
    protected float timeOfMovement;
    protected float error = 0.01f;
    protected bool move;
    //Testing (not intended for use in actual game unless decided otherwise, then move up above)
    public static GridCell testCell;
    private static Entity _instance;
    public static Entity Instance {
        get {
            if (_instance == null) {
                _instance = new();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Vector3 vec = transform.rotation * Vector3.down;
        if (Physics.Raycast(collider.bounds.center, vec, out RaycastHit hit, Mathf.Infinity, 1 << 3))
        {
            Debug.Log("Hits!");
            occupiedCell = hit.transform.GetComponent<GridCell>();
            testCell = occupiedCell;
        }
        height = collider.bounds.size.z;
        offset = new Vector3(0, 0, -height / 2);
        move = false;
        Debug.Log("End of Entity Start!");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
    }
    protected virtual void FixedUpdate() {
        
    }

    public void CalculateTime() {
        float distance = Vector3.Distance(transform.position, OffsetGridPos);
        timeOfMovement = distance / moveSpeed;
        time = 0;
    }
    public virtual GridPath FindPath(GridCell target) {
        return Pathfinder.FindPath(occupiedCell, target);
    }

    public virtual void Move(GridCell target) {
        ArgumentNullExceptionUse.ThrowIfNull(target);

        GridPath path = FindPath(target);
        Move(path);
    }
    public virtual void Move(GridPath path) {
        ArgumentNullExceptionUse.ThrowIfNull(path);
        linkedPath = path;
        occupiedCell = path.PopFront();
        CalculateTime();
        move = true;
    }
    public virtual void Move() {
        if (!move) {
            return;
        }
        if (time / timeOfMovement > .9 && Vector3.Distance(transform.position, OffsetGridPos) < error) {
            if (linkedPath == null) {
                move = false;
                return;
            } else if (linkedPath.Count == 0) {
                move = false;
                prevOccupiedCell = occupiedCell;
                return;
            } else if (linkedPath.Count > 0) {
                prevOccupiedCell = occupiedCell;
                occupiedCell = linkedPath.PopFront();
                CalculateTime();
            }
        }

        transform.position = Vector3.Lerp(OffsetPrevGridPos, OffsetGridPos, time / timeOfMovement);
        time += Time.fixedDeltaTime;
    }
}
