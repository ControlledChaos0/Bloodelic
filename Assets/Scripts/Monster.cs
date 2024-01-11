using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class Monster : Entity
{
    //Testing
    private static Monster _mInstance;
    public static Monster MInstance {
        get {
            if (_mInstance == null) {
                _mInstance = new();
            }
            return _mInstance;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //GameController.Instance.SetMonster(this);
        Pathfinder.FindPath(occupiedCell, occupiedCell);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
