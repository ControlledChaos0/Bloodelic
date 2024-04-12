using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SmallObject : InterObject
{
    public override bool BlockCells {
        get => false;
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
