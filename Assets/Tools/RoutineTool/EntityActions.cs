using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using CJUtils;
#endif

[System.Serializable]
public abstract class EntityAction {
#if UNITY_EDITOR
    public abstract void ShowGUI();
    public virtual void ShowSceneGUI(int actionIndex) { }
    public virtual string ToString()
    {
        return "";
    }
#endif
}

[System.Serializable]
public class ActionMove : EntityAction {
    public GridCell targetCell;
    public GridCellPosition staticPosition;
    public ActionMove() => targetCell = null;
    public ActionMove(GridCell targetCell) => this.targetCell = targetCell;
    public override string ToString()
    {
        if (targetCell != null)
        {
            return "Move to: " + targetCell.name;
        }

        return "Move to: null";
    }

#if UNITY_EDITOR

    bool retargeting; 
    GridCell uncommittedCell;
    
    public override void ShowGUI() {
        EditorUtils.WindowBoxLabel("Target Grid");
        EditorGUIUtility.labelWidth = EditorUtils.MeasureTextWidth("Target Cell", GUI.skin.font) + 15;
        uncommittedCell = EditorGUILayout.ObjectField("Target Cell", uncommittedCell == null
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
            GUI.enabled = !ReferenceEquals(targetCell, uncommittedCell);
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Apply")) {
                    targetCell = uncommittedCell;
                    staticPosition = targetCell.Position;
                    retargeting = false;
                }
                GUI.enabled = true;
                if (GUILayout.Button("Cancel")) {
                    uncommittedCell = targetCell;
                    retargeting = false;
                }
            }
        }
        EditorGUILayout.Space();
        EditorUtils.WindowBoxLabel("Static Position");
        if (staticPosition != null) {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                GUILayout.FlexibleSpace();
                GUILayout.Label($"<b>X:</b>   {staticPosition.Position.x}", new GUIStyle(GUI.skin.label) { richText = true });
                GUILayout.FlexibleSpace();
                GUILayout.Label($"<b>Y:</b>   {staticPosition.Position.y}", new GUIStyle(GUI.skin.label) { richText = true });
                GUILayout.FlexibleSpace();
                GUILayout.Label($"<b>Z:</b>   {staticPosition.Position.z}", new GUIStyle(GUI.skin.label) { richText = true });
                GUILayout.FlexibleSpace();
            }
        } else {
            EditorGUILayout.HelpBox("Static Position Not Assigned;", MessageType.Warning);
        }
    }

    public override void ShowSceneGUI(int actionIndex) {
        if (targetCell == null) return;
        Handles.color = this == RoutineToolGUI.mainWindow.SelectedAction ? Color.red : new Color(0.92f, 0.5f, 0.32f, 0.8f);
        Handles.DrawSolidDisc(targetCell.transform.position, Vector3.up, 0.4f);
        Handles.Label(targetCell.transform.position, $"<b>{actionIndex}</b>", new GUIStyle(GUI.skin.label) { richText = true,
                                                                                                             fontSize = 20,
                                                                                                             alignment = TextAnchor.MiddleCenter});
        Handles.color = Color.white;
    }
    #endif
}

[System.Serializable]
public class ActionWait : EntityAction {
    public int waitTime;
    public ActionWait() => waitTime = 0;
    public ActionWait(int waitTime) => this.waitTime = waitTime;

    public override string ToString()
    {
        return "Wait: " + waitTime;
    }

#if UNITY_EDITOR
    public override void ShowGUI() {
        EditorUtils.WindowBoxLabel("Wait Duration");
        EditorGUIUtility.labelWidth = 48;
        waitTime = EditorGUILayout.IntField("Turns:", waitTime);
        EditorGUIUtility.labelWidth = 0;
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
