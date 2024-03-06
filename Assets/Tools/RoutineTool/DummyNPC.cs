using System.Collections.Generic;
using UnityEngine;

public class DummyNPC : Entity {

    [SerializeReference] private List<EntityAction> routine;
    public List<EntityAction> Routine {
        get {
            if (routine == null) routine = new();
            return routine;
        }
    }

    // Keeps track of which action in the routine should the NPC execute, starts at -1
    private int routineActionIndex = -1;
    
    public EntityAction GetNextActionInRoutine()
    {
        if (Routine.Count == 0)
        {
            Debug.LogWarning("NPC " + name + " has no routines!");
            return null;
        }

        routineActionIndex = (routineActionIndex + 1) % routine.Count;
        return routine[routineActionIndex];
    }

}