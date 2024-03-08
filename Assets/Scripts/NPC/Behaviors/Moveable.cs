using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class Moveable : Behavior
{
    public string Name {
        get => name = "Move";
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartBehaviorAction += StartBehavior;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void StartBehavior()
    {
        base.StartBehavior();
    }
    public override void CheckValid()
    {
        throw new System.NotImplementedException();
    }
}
