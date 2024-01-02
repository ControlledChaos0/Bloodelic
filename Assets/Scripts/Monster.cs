using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class Monster : Entity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Pathfinder.FindPath(_occupiedCell, _occupiedCell);
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
