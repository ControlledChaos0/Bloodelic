using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using CJUtils;
#endif

public class DummyNPC : Entity {

    [SerializeField] private List<EntityAction> routine;
    public List<EntityAction> Routine {
        get {
            if (routine == null) routine = new();
            return routine;
        }
    }
}

public abstract class EntityAction {
    #if UNITY_EDITOR
    public abstract void ShowGUI();
    #endif
}

[System.Serializable]
public class ActionMove : EntityAction {
    public GridCell targetCell;
    public ActionMove() => targetCell = null;
    public ActionMove(GridCell targetCell) => this.targetCell = targetCell;

    #if UNITY_EDITOR

    bool retargeting; 
    GridCell uncommittedCell;
    
    public override void ShowGUI() {
        EditorGUIUtility.labelWidth = EditorUtils.MeasureTextWidth("Target Cell", GUI.skin.font) + 15;
        targetCell = EditorGUILayout.ObjectField("Target Cell", uncommittedCell == null
                                                                ? targetCell : uncommittedCell, typeof(GridCell), true) as GridCell;
        EditorGUIUtility.labelWidth = 0;
        if (retargeting) {
            if (Selection.objects.Length > 0 && Selection.objects[0] is GameObject) {
                GridCell cell = (Selection.objects[0] as GameObject).GetComponentInParent<GridCell>();
                if (cell != null) uncommittedCell = cell;
            }
        }
        if (!retargeting && targetCell == uncommittedCell) {
            if (GUILayout.Button("Retarget Cell")) {
                retargeting = true;
            }
        }
        if (retargeting) {
            GUI.enabled = targetCell != uncommittedCell;
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Apply")) {
                    targetCell = uncommittedCell;
                    retargeting = false;
                }
                if (GUILayout.Button("Revert")) {
                    uncommittedCell = targetCell;
                    retargeting = false;
                }
            }
            GUI.enabled = true;
        }
    }
    #endif
}

[System.Serializable]
public class ActionWait : EntityAction {
    public int waitTime;
    public ActionWait() => waitTime = 0;
    public ActionWait(int waitTime) => this.waitTime = waitTime;

    #if UNITY_EDITOR
    public override void ShowGUI() {
        
    }
    #endif
}

[System.Serializable]
public class ActionInteract : EntityAction {
    #if UNITY_EDITOR
    public override void ShowGUI() {

    }
    #endif
}

[System.Serializable]
public class ActionLookAt : EntityAction {
    #if UNITY_EDITOR
    public override void ShowGUI() {

    }
    #endif
}
