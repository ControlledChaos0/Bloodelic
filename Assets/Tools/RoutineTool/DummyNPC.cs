using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DummyNPC : Entity {

    [SerializeField] private EntityAction[] routine;
}

/// <summary>
/// Class to link the Routine Tool to the NPCs in the scene;
/// <br></br> Has no runtime value; thus, it is destroyed;
/// </summary>
public class NPCManager : MonoBehaviour {
    public T[] LoadEntities<T>() where T : Entity => GetComponentsInChildren<T>(true);
    void OnEnable() => Destroy(this);
}

[CustomEditor(typeof(NPCManager))]
public class NPCManagerEditor : Editor {
    public override void OnInspectorGUI() {
        NPCManager npcManager = target as NPCManager;
        RoutineToolGUI.LaunchRoutineTool(npcManager);
    }
}

public abstract class EntityAction {}

[System.Serializable]
public class ActionMove : EntityAction {
    public GridCell targetCell;
    public ActionMove() => targetCell = null;
    public ActionMove(GridCell targetCell) => this.targetCell = targetCell;
}

[System.Serializable]
public class ActionWait : EntityAction {
    public int waitTime;
    public ActionWait() => waitTime = 0;
    public ActionWait(int waitTime) => this.waitTime = waitTime;
}

[System.Serializable]
public class ActionInteract : EntityAction {
    
}

[System.Serializable]
public class ActionLookAt : EntityAction {

}
