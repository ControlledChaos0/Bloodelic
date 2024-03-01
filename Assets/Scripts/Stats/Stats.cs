using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used as a component belong to each entity
//  Contains stats such as
//  - Movement
//  - Action Points
//  - HP?
public class Stats : MonoBehaviour
{
    #region Stats

    [SerializeField] private int movement = 5;
    [SerializeField] private int actions = 1;

    public int Movement
    {
        get => movement;
        set => movement = value;
    }
    
    public int Actions
    {
        get => actions;
        set => actions = value;
    }
    
    #endregion
}
