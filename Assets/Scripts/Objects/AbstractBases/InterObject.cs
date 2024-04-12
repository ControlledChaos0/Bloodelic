using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BehaviorController))]
public abstract class InterObject : Occupant
{
    public abstract string Name {
        get;
    }
    public abstract string Description {
        get;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    
}
