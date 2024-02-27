using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior : MonoBehaviour
{
    //no idea if we actually need it, temp variable
    private BehaviorController controller;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        controller = GetComponent<BehaviorController>();
        controller.AddBehavior(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public abstract void StartBehavior();
}
