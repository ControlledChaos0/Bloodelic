using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : SmallHoldableObject
{
    public override string Name {
        get => "Bottle";
    }
    public override string Description {
        get => "blah blah blah blah";
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
