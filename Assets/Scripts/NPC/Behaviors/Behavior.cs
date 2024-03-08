using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Behavior : MonoBehaviour
{
    //no idea if we actually need it, temp variable
    protected BehaviorController controller;
    protected Button behaviorButton;
    protected string name;
    public string Name {
        get => name;
    }

    public Button BehaviorButton {
        get => behaviorButton;
        set => behaviorButton = value;
    }

    public UnityAction StartBehaviorAction;
    protected virtual void Awake() {
        controller = GetComponent<BehaviorController>();
        controller.AddBehavior(this);
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void StartBehavior() {
        
    }
    public abstract bool CheckValid();
}
