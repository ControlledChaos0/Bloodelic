using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmallHoldableObject : SmallObject
{
    public new string name = "New Item";
    public string description = "bluhbluhbluh";
    public Sprite icon;
    public static event Action<SmallHoldableObject> pickedUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gettingPickedUp()
    {
        pickedUp?.Invoke(this);
    }


}
