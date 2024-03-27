using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Behavior : MonoBehaviour
{
    protected BehaviorRoutine behaviorRoutine;
    protected Button behaviorButton;
    protected string name;
    public abstract string Name {
        get;
    }
    public abstract bool NPCInteract {
        get;
    }
    public Button BehaviorButton {
        get => behaviorButton;
        set => behaviorButton = value;
    }
    public UnityAction StartBehaviorAction;

    protected virtual void Start()
    {
        
    }

    public virtual void InitializeBehavior() {
        SetRoutine();
    }
    public virtual void ResetBehavior() {
        InitializeBehavior();
        ResetBehaviorSpecifics();
    }

    protected virtual void SetRoutine() {
        behaviorRoutine = CreateRoutine();
        behaviorRoutine.Behavior = this;
        StartBehaviorAction += behaviorRoutine.StartBehaviorRoutine;
    }
    public abstract BehaviorRoutine CreateRoutine();
    public abstract bool CheckValid();
    public abstract void ResetBehaviorSpecifics();
}
