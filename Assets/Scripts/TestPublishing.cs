using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPublishing : MonoBehaviour,
    ISubscriber<GridCellPosition, LineOfSight.ItemSpotted>//,
    //ISubscriber<Entity, GridCell>
{
    public LevelGrid grid;
    public LineOfSight lineOfSight;
    // Start is called before the first frame update
    void Start()
    {
        lineOfSight.detectionEvent.AddListener(ReceiveMessage);
        /*
        foreach (Transform child in grid.transform)
        {
            if (child.GetComponent<GridCell>() != null)
            {
                child.GetComponent<GridCell>().ItemMoved.AddListener(ReceiveMessage);
            }
        }
        */
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveMessage(GridCellPosition g, LineOfSight.ItemSpotted i) {
        Debug.Log(g);
        Debug.Log(i);
    }
    /*
    public void ReceiveMessage(Entity e, GridCell g)
    {
        Debug.Log("Moved " + e.ToString() + " to " + g.Position);
    }
    */
}
