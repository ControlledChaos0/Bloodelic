using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InterObject : Occupant
{
    private Selectable _selectable;
    public abstract string Name {
        get;
    }
    public abstract string Description {
        get;
    }
    public Selectable Selectable {
        get => _selectable;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        _selectable = GetComponent<Selectable>();
        base.Start();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    
}
