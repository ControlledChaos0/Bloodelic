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
    public string Name {
        get => name;
    }

    public Button BehaviorButton {
        get => behaviorButton;
        set => behaviorButton = value;
    }

    public UnityAction StartBehaviorAction;

    protected virtual void SetRoutine(BehaviorRoutine routine) {
        behaviorRoutine = routine;
        behaviorRoutine.Behavior = this;
        StartBehaviorAction += behaviorRoutine.StartBehavior;
    }
    public abstract bool CheckValid();
}
