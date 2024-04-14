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
    public abstract bool Resetable {
        get;
    }
    public Button BehaviorButton {
        get => behaviorButton;
        set => behaviorButton = value;
    }
    public UnityAction StartBehaviorAction;


    public virtual void InitializeBehavior() {
        SetRoutine();
        ResetBehaviorSpecifics();
    }

    public virtual void ExecuteBehavior() {
        behaviorRoutine.ExecuteBehaviorRoutine();
        StartCoroutine(ExecuteBehaviorCoroutine());
    }

    protected virtual void SetRoutine() {
        if (behaviorRoutine != null) {
            StartBehaviorAction -= behaviorRoutine.StartBehaviorRoutine;
        }
        behaviorRoutine = CreateRoutine();
        behaviorRoutine.Behavior = this;
        StartBehaviorAction += behaviorRoutine.StartBehaviorRoutine;
    }
    public abstract BehaviorRoutine CreateRoutine();
    public abstract bool CheckValid();
    public abstract void ResetBehaviorSpecifics();
    public abstract IEnumerator ExecuteBehaviorCoroutine();
}
