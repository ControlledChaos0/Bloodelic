using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

public class RoutineToolGUI : EditorWindow {

    public static RoutineToolGUI mainWindow;

    private Vector2 npcScroll;
    private Vector2 routineScroll;

    public static void LaunchRoutineTool(NPCManager npcManager) {
        mainWindow = GetWindow<RoutineToolGUI>("NPC Routine Tool");
        mainWindow.Init(npcManager);
    }

    private NPCManager npcManager;
    private DummyNPC[] registeredNPCs;

    private DummyNPC selectedNPC;
    private EntityAction selectedAction;

    private void Init(NPCManager npcManager) {
        this.npcManager = npcManager;
        registeredNPCs = npcManager.LoadEntities<DummyNPC>();
    }

    void OnGUI() {
        using (var scope = new EditorGUI.ChangeCheckScope()) {
            using (new EditorGUILayout.HorizontalScope()) {
                DrawGUI();
            }
        }
    }

    private void DrawGUI() {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(position.width * 2f/7f))) {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorUtils.WindowBoxLabel("NPCs", UIStyles.CenteredLabelBold, GUILayout.Height(18));
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    if (GUILayout.Button(EditorUtils.FetchIcon("d_Refresh"), EditorStyles.miniButton, GUILayout.Width(30))) {
                        registeredNPCs = npcManager.LoadEntities<DummyNPC>();
                    }
                }
            }
            using (new EditorGUILayout.VerticalScope(EditorStyles.textArea)) {
                using (var scope = new EditorGUILayout.ScrollViewScope(npcScroll, UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                    npcScroll = scope.scrollPosition;
                    foreach (DummyNPC npc in registeredNPCs) {
                        DrawButton(npc);
                    }
                }
            }
        }
        using (new EditorGUILayout.VerticalScope()) {
            EditorUtils.WindowBoxLabel(selectedNPC == null ? "No NPC Selected" : selectedNPC.gameObject.name, UIStyles.CenteredLabelBold, GUILayout.Height(18));
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope(GUILayout.MinWidth(position.width * 2f / 7f))) {
                    EditorUtils.WindowBoxLabel("NPC Routine", UIStyles.CenteredLabelBold);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.textArea)) {
                        if (selectedNPC == null) {
                            EditorUtils.DrawScopeCenteredText("Select an NPC from the List;\n" +
                                                              "Its routine will appear here;");
                        } else {
                            using (var scope = new EditorGUILayout.ScrollViewScope(routineScroll, UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                                routineScroll = scope.scrollPosition;
                                if (selectedNPC.Routine.Count == 0) {
                                    EditorUtils.DrawScopeCenteredText("- Empty -");
                                } else {
                                    for (int i = 0; i < selectedNPC.Routine.Count; i++) {
                                        DrawButton(i, selectedNPC.Routine[i]);
                                    }
                                }
                            }
                        }
                    }
                }
                using (new EditorGUILayout.VerticalScope()) {
                    EditorUtils.WindowBoxLabel("Action Info");
                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                        if (selectedNPC == null) {
                            EditorUtils.DrawScopeCenteredText("No NPC Selected;");
                        } else {
                            if (selectedAction == null) {
                                EditorUtils.DrawScopeCenteredText("No Action Selected;");
                            } else {
                                selectedAction.ShowGUI();
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                    EditorUtils.WindowBoxLabel("Routine Controls");
                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                        if (selectedNPC == null) {
                            EditorUtils.DrawScopeCenteredText("No NPC Selected;");
                        } else {
                            if (GUILayout.Button("Add Move Action")) {
                                selectedNPC.Routine.Add(new ActionMove());
                            }
                            if (GUILayout.Button("Add Wait Action")) {
                                selectedNPC.Routine.Add(new ActionWait());
                            }
                            GUI.enabled = false; /// Not Implemented!
                            if (GUILayout.Button("Add Interact Action")) {
                                selectedNPC.Routine.Add(new ActionInteract());
                            }
                            if (GUILayout.Button("Add Look At Action")) {
                                selectedNPC.Routine.Add(new ActionLookAt());
                            }
                            GUI.enabled = true;
                        }
                    }
                }
            }
        }
    }

    private void DrawButton(DummyNPC npc) {
        if (GUILayout.Button(npc.gameObject.name, selectedNPC == npc
                                        ? new GUIStyle(EditorStyles.numberField) {
                                            alignment = TextAnchor.MiddleCenter,
                                            normal = { textColor = UIColors.Blue } }
                                        : GUI.skin.button)) {
            selectedAction = null;
            selectedNPC = npc;
        }
    }

    private void DrawButton(int index, EntityAction action) {
        using (new EditorGUILayout.HorizontalScope()) {
            GUI.color = UIColors.Red;
            if (GUILayout.Button(EditorUtils.FetchIcon("d_TreeEditor.Trash"), GUILayout.Width(32))) {
                if (selectedAction == action) selectedAction = null;
                selectedNPC.Routine.RemoveAt(index);
            } GUI.color = Color.white;
            if (GUILayout.Button(action.GetType().Name, selectedAction == action
                                              ? new GUIStyle(EditorStyles.numberField) {
                                                  alignment = TextAnchor.MiddleCenter,
                                                  normal = { textColor = UIColors.Blue } }
                                              : GUI.skin.button)) {
                selectedAction = action;
            }
            if (index == 0) GUI.enabled = false;
            if (GUILayout.Button("▲", GUILayout.Width(24))) {
                selectedNPC.Routine.RemoveAt(index);
                selectedNPC.Routine.Insert(index - 1, action);
            } GUI.enabled = true;
            if (index == selectedNPC.Routine.Count - 1) GUI.enabled = false;
            if (GUILayout.Button("▼", GUILayout.Width(24))) {
                selectedNPC.Routine.RemoveAt(index);
                selectedNPC.Routine.Insert(index + 1, action);
            } GUI.enabled = true;
        }
    }
}