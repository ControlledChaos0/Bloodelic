using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalLightDemoMovement : MonoBehaviour
{
    private Quaternion initialRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Sin(Time.time) * 15, transform.up) * initialRotation;
    }
}
