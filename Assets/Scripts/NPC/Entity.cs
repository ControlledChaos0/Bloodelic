using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

[RequireComponent(typeof(BehaviorController))]
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
    protected Selectable selectable;
    protected SplineContainer splineContainer;
    protected Spline currSpline;
    protected Vector3 GroundPosition => transform.position + (transform.rotation * offset);
    protected Vector3 OffsetGridPos => occupiedCell.transform.position + (occupiedCell.transform.rotation * -offset);
    protected Vector3 OffsetPrevGridPos => prevOccupiedCell.transform.position + (prevOccupiedCell.transform.rotation * -offset);
    protected GridPath linkedPath;
    protected GridCell prevOccupiedCell;
    protected Quaternion fromRot;
    protected Quaternion toRot;
    protected Vector3 offset;
    protected float height;
    protected float timeOfMovement;
    protected float timeOfRotate;
    protected float error = 0.01f;
    protected bool move;

    protected Spline spline;
    //Testing (not intended for use in actual game unless decided otherwise, then move up above)
    public static GridCell testCell;

    public bool IsMoving {
        get { return move; }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        selectable = GetComponent<Selectable>();

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

    public virtual void Select() {
        return;
    }
    public virtual void HoverSelect(GameObject gO) {
        return;
    }

    public virtual GridPath FindPath(GridCell target)
    {
        Pathfinder.moveLimit = Mathf.Infinity;
        Pathfinder.entity = this;
        return Pathfinder.FindPath(occupiedCell, target);
    }
    
    public virtual GridPath FindPathWithMoveLimit(GridCell target)
    {
        Pathfinder.moveLimit = Movement;
        Pathfinder.entity = this;
        return Pathfinder.FindPath(occupiedCell, target, true);
    }

    public virtual void Move(GridCell target) {
        ArgumentNullExceptionUse.ThrowIfNull(target);

        GridPath path = FindPath(target);
        Move(path);
    }
    
    // This version is used by NPCs and tells the game to wait until the movement is complete
    public IEnumerator MoveCoroutine(GridCell target)
    {
        ArgumentNullExceptionUse.ThrowIfNull(target);

        Debug.Log(name + " begins moving to " + target.name);
        
        GridPath path = FindPath(target);
        ArgumentNullExceptionUse.ThrowIfNull(path);

        splineContainer = SplinePathCreator.CreateSplinePath(path);
        if (splineContainer == null)
        {
            yield break;
        }
        
        linkedPath = path;
        linkedPath.RevertColor();
        
        yield return StartCoroutine(IterateThroughSpline());
    }
    
    public virtual void Move(GridPath path) {
        ArgumentNullExceptionUse.ThrowIfNull(path);

        splineContainer = SplinePathCreator.CreateSplinePath(path);

        linkedPath = path;
        linkedPath.RevertColor();
        
        StartCoroutine(IterateThroughSpline());
        Debug.Log("MOVE IDIOT");
    }

    public void SetOccupation(GridCell cell)
    {
        if (cell == null) { return; }
        occupiedCell.Unoccupy();
        occupiedCell = cell;
        // Set occupant for occupied cell
        occupiedCell.SetOccupant(this);
        PostGridMovement();
    }

    // Things that need to occur after entity occupies a new cell, implement in child classes
    protected virtual void PostGridMovement() {}
    
    private IEnumerator IterateThroughSpline() {
        int splineCount = splineContainer.Splines.Count;
        Vector3 pathStartPos = splineContainer.gameObject.transform.position;
        linkedPath.PopFront();

        for (int i = 0; i < splineCount; i++) {
            SetOccupation(linkedPath.PopFront());
            Spline currSpline = splineContainer.Splines[i];

            float t = 0;
            // Quaternion start = transform.rotation;
            // Quaternion end = currSpline[0].Rotation;
            // float rotateTime = Quaternion.Angle(start, end) / rotateSpeed;
            // while (t < rotateTime) {
            //     transform.rotation = Quaternion.Lerp(start, end, t / rotateTime);
            //     t += Time.fixedDeltaTime;
            //     yield return new WaitForFixedUpdate();
            // }

            t = 0;
            float moveTime = currSpline.GetLength() / moveSpeed;
            float3 pos;
            float3 tangent;
            float3 upward;
            while (t < moveTime) {
                currSpline.Evaluate(t / moveTime, out pos, out tangent, out upward);
                transform.position = (Vector3)pos + pathStartPos;
                transform.up = upward;
                transform.forward = tangent;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        Destroy(splineContainer.gameObject);
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
