using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SmallHoldable))]
public abstract class SmallHoldableObject : SmallObject
{
    public Sprite icon;
    public static event Action<SmallHoldableObject> pickedUp;

    [Header("Behaviors")]
    [SerializeField]
    protected SmallHoldable smallHoldable;

    public SmallHoldable SmallHoldable {
        get => smallHoldable;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        smallHoldable.Object = this;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GettingPickedUp()
    {
        pickedUp?.Invoke(this);
    }


}
