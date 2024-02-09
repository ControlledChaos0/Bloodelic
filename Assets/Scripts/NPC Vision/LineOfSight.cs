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

    private Cone coneScript;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(DetectEntity(player));
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
    public bool DetectEntity(GameObject entity, float viewAngle = ANGLE)
    {
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position,
            (entity.transform.position - transform.position).normalized,
            out hit
        ))
        {
            if (hit.transform.Equals(entity.transform))
            {
                Debug.Log(Vector3.Angle((entity.transform.position - transform.position).normalized, transform.forward));
                if (Vector3.Angle(
                    (entity.transform.position - transform.position).normalized,
                    transform.forward) < viewAngle)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //TODO: Method is incomplete
    //Will be a helper for when the npc is clicked
    private void RevealCone()
    {
        coneScript = cone.GetComponent<Cone>();
        coneScript.ConeHeight = CONE_HEIGHT;
        coneScript.ConeRadius = ((float)(coneScript.ConeHeight * Math.Tan(ANGLE * (Math.PI / 180))));
    }




}
