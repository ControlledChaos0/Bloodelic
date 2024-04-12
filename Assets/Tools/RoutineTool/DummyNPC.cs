using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DummyNPC : Entity {

    [SerializeReference] private List<EntityAction> routine;
    public List<EntityAction> Routine {
        get {
            if (routine == null) routine = new();
            return routine;
        }
    }
    public override bool BlockCells {
        get => true;
    }
    
    protected AIBrain aiBrain { get; set; }
    
    public List<MapExit> mapExits = new List<MapExit>();
    public Dictionary<GridCell, MapExit> gridCellMapExitsDictionary = new Dictionary<GridCell, MapExit>();
    
    protected IEnumerator currentCoroutine;

    protected virtual void Start()
    {
        base.Start();
        aiBrain = GetComponent<AIBrain>();
        
        // Cache all map exits
        FindMapExits();
        
        // Update state once after entity is fully initialized
        aiBrain.UpdateCurrentState();
    }
    
    // Keeps track of which action in the routine should the NPC execute, starts at -1
    private int routineActionIndex = -1;
    EntityAction nextAction;

    public EntityAction NextAction
    {
        get => nextAction;
        set => nextAction = value;
    }
    
    public EntityAction GetNextActionInRoutine()
    {
        if (Routine.Count == 0)
        {
            Debug.LogWarning("NPC " + name + " has no routines!");
            return null;
        }
        
        // If there is already an action assigned, check if it is still valid
        if (IsActionValid(nextAction))
        {
            return nextAction;
        }

        // Find next valid action
        routineActionIndex = GetNextValidActionIndex();
        if (routineActionIndex == -1)
        {
            return null;
        }
        
        return routine[routineActionIndex];
    }

    // Iterates through current routine and finds the next valid Action's index to perform
    //  returns -1 if none is found
    public int GetNextValidActionIndex()
    {
        for (int i = 1; i <= routine.Count; i++)
        {
            int nextIndex = (routineActionIndex + i) % routine.Count;
            EntityAction nextAction = routine[nextIndex];
            // Verify if action is valid
            if (IsActionValid(nextAction))
            {
                return nextIndex;
            }
        }

        // If none is found
        return -1;
    }

    bool IsActionValid(EntityAction action)
    {
        if (action == null)
        {
            return false;
        }
        
        // ActionMove is only valid if current location is different from target cell
        if (action is ActionMove)
        {
            ActionMove moveAction = (ActionMove)action;
            if (occupiedCell != moveAction.targetCell &&
                !moveAction.targetCell.IsOccupied())
            {
                return true;
            }
        }
        // Not implemented
        else if (action is ActionInteract)
        {
            return false;
        }
        else if (action is ActionWait)
        {
            return true;
        }

        return false;
    }
    
    public void FindMapExits()
    {
        mapExits = FindObjectsOfType<MapExit>().ToList();

        foreach (var e in mapExits)
        {
            if (!e.isInitialized)
            {
                e.RegisterGridCell();
            }
            
            gridCellMapExitsDictionary.Add(e.cell, e);
        }
    }
    
    public void StopCurrentBehaviorCoroutine()
    {
        StopCoroutine(currentCoroutine);
    }
}