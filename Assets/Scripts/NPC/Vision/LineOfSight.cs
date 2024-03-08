using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using KevinCastejon.ConeMesh;

public class LineOfSight : MonoBehaviour
{
    //Viewing angle for line-of-sight
    private const float ANGLE = 45;

    //Vars for scanning environment for the line-of-sight visual
    private const int scanFreq = 20;
    private float scanTimer;
    private float scanInterval;

    [SerializeField]
    private GameObject player;
    public bool canSeePlayer;

    //For detection of the tiles
    private const float OVERLAP_SPHERE_RADIUS = 50;
    private List<GameObject> tileList = new List<GameObject>();

    //Basic state machine for showing/hiding line of sight
    private enum SightLineShowState
    {
        REVEALSIGHT,
        HIDESIGHT
    }
    private SightLineShowState state = SightLineShowState.HIDESIGHT;
    private SightLineShowState prevState = SightLineShowState.HIDESIGHT;

    void OnDestroy()
    {
        //HumanManager.Instance.ClickAction -= OnClick;
    }

    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1.0f / scanFreq;
        //HumanManager.Instance.ClickAction += OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0.0f)
        {
            scanTimer += scanInterval;
            canSeePlayer = DetectEntitySight(player, ANGLE);
            //Debug.Log(canSeePlayer);
            if (state == SightLineShowState.REVEALSIGHT)
            {
                OnRevealSightLine();
                prevState = SightLineShowState.REVEALSIGHT;
            }
            else
            {
                if (prevState != SightLineShowState.HIDESIGHT)
                {
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
    public bool DetectEntitySight(GameObject entity, float viewAngle = ANGLE, int mask = Physics.AllLayers)
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
        {
            if (hit.transform.Equals(entity.transform))
            {
                //Debug.Log(Vector3.Angle((entity.transform.position - transform.position).normalized, transform.forward));
                if (Vector3.Angle(
                    (entity.transform.position - transform.position).normalized,
                    transform.forward) <= viewAngle)
                {
                    return true;
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
        overlap = Physics.OverlapSphere(transform.position, OVERLAP_SPHERE_RADIUS, LayerMask.GetMask("Grid"), QueryTriggerInteraction.Ignore);
        Debug.Log(overlap.Length);
        //tilesInSight.Clear();
        for(int i = tileList.Count - 1; i >= 0; i--)
        {
            if (Array.IndexOf(overlap, tileList[i]) == -1)
            {
                if (tileList[i].GetComponent<GridCell>() != null)
                    tileList[i].GetComponent<GridCell>().RevertColor();
                tileList.RemoveAt(i);
            }
        }
        foreach(Collider c in overlap)
        {
            if (DetectEntitySight(c.gameObject, ANGLE, LayerMask.GetMask("Grid")) && c.gameObject.GetComponent<GridCell>() != null)
            {
                if (!tileList.Contains(c.gameObject))
                {
                    c.gameObject.GetComponent<GridCell>().TurnBlue();
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
                tileList[i].GetComponent<GridCell>().RevertColor();
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
}
