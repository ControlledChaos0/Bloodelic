using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

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
    public Sprite icon;
    protected SplineAnimate splineAnimate;
    protected Vector3 GroundPosition => transform.position + (transform.rotation * offset);
    protected Vector3 OffsetGridPos => occupiedCell.transform.position + (occupiedCell.transform.rotation * -offset);
    protected Vector3 OffsetPrevGridPos => prevOccupiedCell.transform.position + (prevOccupiedCell.transform.rotation * -offset);
    protected GridPath linkedPath;
    protected GridCell prevOccupiedCell;
    protected Quaternion fromRot;
    protected Quaternion toRot;
    protected Vector3 offset;
    protected float height;
    protected float moveTime = 0;
    protected float rotateTime = 0;
    protected float timeOfMovement;
    protected float timeOfRotate;
    protected float error = 0.01f;
    protected bool move;

    protected Spline spline;
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

    public bool IsMoving {
        get { return move; }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        splineAnimate.MaxSpeed = moveSpeed;

        Vector3 vec = transform.rotation * Vector3.down;
        if (Physics.Raycast(collider.bounds.center, vec, out RaycastHit hit, Mathf.Infinity, 1 << 3))
        {
            Debug.Log("Hits!");
            occupiedCell = hit.transform.GetComponent<GridCell>();
            prevOccupiedCell = occupiedCell;
            testCell = occupiedCell;
            // Set occupant for occupied cell
            occupiedCell.SetOccupant(this);
        }
        height = collider.bounds.size.z;
        offset = new Vector3(0, -height / 2, 0);
        move = false;
        
        // Stats reference
        stats = GetComponent<Stats>();
        
        Debug.Log("End of Entity Start!");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Move();
    }
    protected virtual void FixedUpdate() {
        
    }

    private void CalculateMovementTime() {
        float distance = Vector3.Distance(transform.position, OffsetGridPos);
        timeOfMovement = distance / moveSpeed;
        moveTime = 0;
    }
    private void CalculateRotateTime() {
        float angle = Vector3.Angle((OffsetGridPos - OffsetPrevGridPos).normalized, transform.forward);
        timeOfRotate = angle / rotateSpeed;
        rotateTime = 0;
    }
    private void CalculateTime() {
        CalculateMovementTime();
        CalculateRotateTime();
    }
    public virtual GridPath FindPath(GridCell target)
    {
        Pathfinder.moveLimit = Mathf.Infinity;
        return Pathfinder.FindPath(occupiedCell, target);
    }
    
    public virtual GridPath FindPathWithMoveLimit(GridCell target)
    {
        Pathfinder.moveLimit = Movement;
        return Pathfinder.FindPath(occupiedCell, target, true);
    }

    public virtual void Move(GridCell target) {
        ArgumentNullExceptionUse.ThrowIfNull(target);

        GridPath path = FindPath(target);
        Move(path);
    }
    public virtual void Move(GridPath path) {
        ArgumentNullExceptionUse.ThrowIfNull(path);

        splineAnimate.Container = SplinePathCreator.CreateSplinePath(path);
        splineAnimate.Play();

        linkedPath = path;
        linkedPath.RevertColor();
        occupiedCell = path.PopFront();
        // Set occupant for occupied cell
        occupiedCell.SetOccupant(this);
        CalculateMovementTime();
        move = true;

        fromRot = transform.rotation;
        toRot = Quaternion.LookRotation(OffsetGridPos - OffsetPrevGridPos, transform.up);
        CalculateRotateTime();
    }
    public virtual void Move() {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, OffsetGridPos - OffsetPrevGridPos, Color.blue);
        if (!move) {
            return;
        }
        if (Rotate()) {
            transform.rotation = Quaternion.Slerp(fromRot, toRot, rotateTime / timeOfRotate);
            rotateTime += Time.fixedDeltaTime;
            return;
        }
        if (Transform()) {
            return;
        }
        if (moveTime / timeOfMovement > .9 && Vector3.Distance(transform.position, OffsetGridPos) < error) {
            if (linkedPath == null) {
                move = false;
                return;
            } else if (linkedPath.Count == 0) {
                move = false;
                prevOccupiedCell.Unoccupy();
                prevOccupiedCell = occupiedCell;
                return;
            } else if (linkedPath.Count > 0) {
                // Unoccupy prev cell
                prevOccupiedCell.Unoccupy();
                prevOccupiedCell = occupiedCell;
                occupiedCell = linkedPath.PopFront();
                // Set occupant for occupied cell
                occupiedCell.SetOccupant(this);
                CalculateMovementTime();
                fromRot = transform.rotation;
                toRot = Quaternion.LookRotation(OffsetGridPos - OffsetPrevGridPos, transform.up);
                CalculateRotateTime();
            }
        }
        transform.position = Vector3.Lerp(OffsetPrevGridPos, OffsetGridPos, moveTime / timeOfMovement);
        moveTime += Time.fixedDeltaTime;
    }

    public virtual bool Transform() {
        return false;
    }
    public virtual bool Rotate() {
        Debug.Log($"Angle between: {Vector3.Angle((OffsetGridPos - OffsetPrevGridPos).normalized, transform.forward)}");
        if (Vector3.Angle((OffsetGridPos - OffsetPrevGridPos).normalized, transform.forward) == 0) {
            return false;
        }
        if (occupiedCell.Equals(prevOccupiedCell)) {
            return false;
        }
        return true;
    }

    private void CreateSplinePath(GridPath path) {

    }
    
    #region Grid Cell

    public GridCell OccupiedCell
    {
        get => occupiedCell;
        set => occupiedCell = value;
    }

    #endregion
    
    #region Stats
    protected Stats stats { get; set; }
    private int defaultMovement = 5;
    public int Movement => (stats != null) ? stats.Movement : defaultMovement;
    

    #endregion 
}
