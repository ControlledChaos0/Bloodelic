using System.Collections.Generic;
using UnityEngine;

public class DummyNPC : Entity {

    [SerializeField] private List<EntityAction> routine;
    public List<EntityAction> Routine {
        get {
            if (routine == null) routine = new();
            return routine;
        }
    }
}