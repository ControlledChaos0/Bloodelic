using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using KevinCastejon.ConeMesh;
using System.Resources;
using Unity.VisualScripting;

public class LineOfSight : MonoBehaviour, ISubscriber<Occupant, GridCell>,
    IPublisher<GridCellPosition, LineOfSight.ItemSpotted>
{
    //Sight subscription/Publishing things
    // Enum of states based on sight
    public enum ItemSpotted {
        NEUTRAL,
        SUSPICION,
        PANICKED
    }

    public ItemSpotted sightState = ItemSpotted.NEUTRAL;

    private LevelGrid grid;
    public DetectionEvent detectionEvent = new DetectionEvent();

    
    //Viewing angle for line-of-sight
    private const float DEFAULT_ANGLE = 45;
    [SerializeField]
    private float _angle = DEFAULT_ANGLE;
    //For detection of the tiles
    private const float DEFAULT_OVERLAP_SPHERE_RADIUS = 50;
    [SerializeField]
    private float _overlapSphereRadius = DEFAULT_OVERLAP_SPHERE_RADIUS;

    [SerializeField, Range(0, 1)]
    private float _viewFalloff = .75f;

    //Vars for scanning environment for the line-of-sight visual
    private const int scanFreq = 5;
    private float scanTimer;
    private float scanInterval;

    [SerializeField]
    private GameObject player;
    public bool canSeePlayer;

    
    private List<GameObject> tileList = new List<GameObject>();
    private List<GridCell> publishers = new List<GridCell>();

    //Basic state machine for showing/hiding line of sight
    private enum SightLineShowState
    {
        REVEALSIGHT,
        HIDESIGHT
    }
    [SerializeField]
    private SightLineShowState state = SightLineShowState.HIDESIGHT;
    private SightLineShowState prevState = SightLineShowState.HIDESIGHT;

    void OnDestroy()
    {
        //HumanManager.Instance.ClickAction -= OnClick;
        foreach (GridCell child in publishers) {
            if (child != null) {
                child.ItemMoved.RemoveListener(ReceiveMessage);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = LevelGrid.Instance;
        scanInterval = 1.0f / scanFreq;
        //HumanManager.Instance.ClickAction += OnClick;
        if (grid != null) {
            foreach (Transform child in grid.transform) {
                if (child.GetComponent<GridCell>() != null) {
                    child.GetComponent<GridCell>().ItemMoved.AddListener(ReceiveMessage);
                    publishers.Add(child.GetComponent<GridCell>());
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        //Debug.Log(sightState);
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0.0f)
        {
            scanTimer += scanInterval;
            canSeePlayer = DetectEntitySight(player, _angle);
            if (canSeePlayer) {
                sightState = ItemSpotted.PANICKED;
                Publish(player.GetComponent<Monster>().OccupiedCell.Position, ItemSpotted.PANICKED);
            }
            //Debug.Log(canSeePlayer);
            if (state == SightLineShowState.REVEALSIGHT)
            {
                Debug.Log("REVEALINGGGGGGGGGG");
                OnRevealSightLine();
                prevState = SightLineShowState.REVEALSIGHT;
            }
            else
            {
                if (prevState != SightLineShowState.HIDESIGHT)
                {
                    Debug.Log("Hidings");
                    ClearTiles();
                }
                prevState = SightLineShowState.HIDESIGHT;
            }
        }
    }

    /**
     * Method for entity detection. 
     * Usage should normally be to detect the player, but I added
     * the ability to detect other things.
     * Will likely need to be integrated into a state machine
     * @param entity - A GameObject to detect
     * @param viewAngle - the viewing angle to check with.
     *      should normally be the same as the angle field of this class.
     * @param mask selects layers that the Raycast can interact with. By default, interacts with all layers.
     * @return bool representing whether the item is within viewing radius.
     */
    public bool DetectEntitySight(GameObject entity, float viewAngle = DEFAULT_ANGLE, int mask = Physics.AllLayers)
    {
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position,
            (entity.transform.position - transform.position).normalized,
            out hit,
            int.MaxValue,
            mask,
            QueryTriggerInteraction.Ignore
        ))
        //Debug.Log(hit.collider);
        {
            if (HasTransform(hit.transform, entity.transform))
            {
                //Debug.Log(Vector3.Angle((entity.transform.position - transform.position).normalized, transform.forward));
                if (Vector3.Angle(
                    (entity.transform.position - transform.position).normalized,
                    transform.forward) <= viewAngle)
                {
                    if ((LayerMask.GetMask("Grid") & mask) != 0) {
                        return Vector3.Dot(entity.transform.up, transform.forward) <= 0;
                    } else {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    

    /**
     * Reveals line of sight for NPC.
     * Detects nearby tiles and turns the ones in its viewing angle blue.
     * TODO: Update this method if we want to do something other than turn tiles blue.
     */
    private void OnRevealSightLine()
    {
        Collider[] overlap = null;
        overlap = Physics.OverlapSphere(transform.position, _overlapSphereRadius, LayerMask.GetMask("Grid"), QueryTriggerInteraction.Ignore);
        Debug.Log(overlap.Length);
        //tilesInSight.Clear();
        for(int i = tileList.Count - 1; i >= 0; i--)
        {
            if (Array.IndexOf(overlap, tileList[i]) == -1)
            {
                if (tileList[i].GetComponent<GridCell>() != null)
                    tileList[i].GetComponent<GridCell>().HideCell();
                tileList.RemoveAt(i);
            }
        }
        foreach(Collider c in overlap)
        {
            if (DetectEntitySight(c.gameObject, _angle, LayerMask.GetMask("Grid")) && c.gameObject.GetComponent<GridCell>() != null)
            {
                if (!tileList.Contains(c.gameObject))
                {
                    c.gameObject.GetComponent<GridCell>().ShowCell();
                    tileList.Add(c.gameObject);
                }
            }
        }

    }

    /**
     * Clears all tiles highlighted by the line of sight visualization.
     * Does nothing if tileList is empty
     */
    public void ClearTiles()
    {
        for (int i = tileList.Count - 1; i >= 0; i--)
        {
            if (tileList[i].GetComponent<GridCell>() != null)
                tileList[i].GetComponent<GridCell>().HideCell();
            tileList.RemoveAt(i);
        }
    }

    /**
     * Updates state when NPC is clicked. 'Nuff said.
     */
    private void OnClick(Human h)
    {
        if (h != null && h.Equals(this.gameObject.GetComponent<Human>()))
        {
            if (state == SightLineShowState.HIDESIGHT)
            {
                state = SightLineShowState.REVEALSIGHT;
            }
            else
            {
                state = SightLineShowState.HIDESIGHT;
            }
        }
        else
        {
            state = SightLineShowState.HIDESIGHT;
        }
    }

    public void ShowSight() {
        if (state == SightLineShowState.HIDESIGHT) {
            state = SightLineShowState.REVEALSIGHT;
        }
    }
    public void HideSight() {
        if (state == SightLineShowState.REVEALSIGHT) {
            state = SightLineShowState.HIDESIGHT;
        }
    }

    /*
     * Implementation of subscriber pattern.
     * Receives messages from GridCells about the current position of an entity.
     * Checks if this entity can be seen and raises suspicion level if needed.
     */
    public void ReceiveMessage(Occupant o, GridCell g)
    {
        //Debug.Log("In ReceiveMessage");
        if (o == null || g == null)
        {
            Debug.Log("Nope");
            return;
        }
        //Debug.Log("Check");
        if (o.gameObject.Equals(player) && DetectEntitySight(o.gameObject, _angle)) {
            //Debug.Log("Perhaps there?");
            sightState = ItemSpotted.PANICKED;
            Publish(g.Position, sightState);
        } else if (HasComponent<Human>(g.gameObject)) {
            //Debug.Log("Certainly not");
            return;
        } else {
            //Debug.Log("Are we here?");
            if (DetectEntitySight(o.gameObject, _angle)) {
                sightState = ItemSpotted.SUSPICION;
                Publish(g.Position, sightState);
            }
            //else
            //{
                //Debug.Log("NAh");
            //}
        }

    }

    bool HasComponent<T>(GameObject g)
    {
        if (GetComponentsInParent<T>() != null && GetComponentsInParent<T>().Length > 0) return true;
        if (GetComponentsInChildren<T>() != null && GetComponentsInChildren<T>().Length > 0) return true;
        return false;
    }

    /*
     * Implementation of Publisher/Subscriber pattern.
     * Publishes a message stating that an item was spotted, where it was,
     * and the suspicion level.
     */
    public void Publish(GridCellPosition g, ItemSpotted i) {
        //Debug.Log("Well, we're at line 242");
        detectionEvent?.Invoke(g, i);
    }

    /*
     * Lowers the suspicion level of this NPC.
     */
    public void LowerSuspicion() {
        if (sightState > 0) sightState--;
    }

    
    /*
     * Checks if a given transform or any of its children or parents matches the given target.
     * Used alongside DetectEntitySight to check for a sightline to objects that are in multiple pieces.
     */
    bool HasTransform(Transform currentTransform, Transform targetTransform)
    {
        if (currentTransform.Equals(targetTransform) || currentTransform.position.Equals(targetTransform.position))
        {
            return true;
        }

        // Check parents
        if (currentTransform.parent != null && currentTransform.GetComponentsInParent<Transform>() != null)
        {
            foreach (Transform t in currentTransform.GetComponentsInParent<Transform>())
            {
                if (t.Equals(targetTransform)) {
                    return true;
                }
            }
        }

        // Check children
        foreach (Transform child in currentTransform)
        {
            if (HasTransform(child, targetTransform))
            {
                return true;
            }
        }

        return false;
    }
}
