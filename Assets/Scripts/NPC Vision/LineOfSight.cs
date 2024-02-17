using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using KevinCastejon.ConeMesh;

public class LineOfSight : MonoBehaviour
{
    private const float ANGLE = 45;
    private const int CONE_HEIGHT = 10;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject cone;

    //For detection of the tiles
    private const float OVERLAP_SPHERE_RADIUS = 50;
    
    private List<GameObject> tileList = new List<GameObject>();



    private Cone coneScript;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OnRevealSightLine();
        Debug.Log(DetectEntitySight(player));
    }

    /**
     * Method for entity detection. 
     * Usage should normally be to detect the player, but I added
     * the ability to detect other things.
     * Will likely need to be integrated into a state machine
     * @param entity - A GameObject to detect
     * @param viewAngle - the viewing angle to check with.
     *      should normally be the same as the angle field of this class.
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

    

    //TODO: Method is incomplete
    //Will be a helper for when the npc is clicked
    private void OnRevealSightLine()
    {
        /*coneScript = cone.GetComponent<Cone>();
        coneScript.ConeHeight = CONE_HEIGHT;
        coneScript.ConeRadius = ((float)(coneScript.ConeHeight * Math.Tan(ANGLE * (Math.PI / 180))));
        */
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

    public void ClearTiles()
    {
        for (int i = tileList.Count - 1; i >= 0; i--)
        {
            if (tileList[i].GetComponent<GridCell>() != null)
                tileList[i].GetComponent<GridCell>().RevertColor();
            tileList.RemoveAt(i);
        }
    }

    IEnumerator ClearTilesAfterFiveSeconds()
    {
        yield return new WaitForSeconds(5);
        ClearTiles();
    }
}
