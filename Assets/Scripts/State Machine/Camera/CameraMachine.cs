using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMachine : StateMachine
{
    //I actually really don't know if I need this, but keeping it for now i guess
    public CameraController cameraController;
    public StateCameraUse cameraUse = new();
    // Start is called before the first frame update
    protected override void Start()
    {
        cameraController = GetComponent<CameraController>();
        StartState(cameraUse);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
