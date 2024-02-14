using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

public class RoutineToolGUI : EditorWindow {

    public static RoutineToolGUI mainWindow;

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
        using (new EditorGUILayout.HorizontalScope()) {
            DrawLeftPanel();
        }
    }

    private void DrawLeftPanel() {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(205))) {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorUtils.WindowBoxLabel("NPCs", UIStyles.CenteredLabelBold, GUILayout.Height(18));
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    if (GUILayout.Button(EditorUtils.FetchIcon("d_Refresh"), EditorStyles.miniButton, GUILayout.Width(30))) {
                        registeredNPCs = npcManager.LoadEntities<DummyNPC>();
                    }
                }
            }
            using (new EditorGUILayout.VerticalScope(EditorStyles.textArea)) {
                using (new EditorGUILayout.ScrollViewScope(Vector2.zero, UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                    foreach (DummyNPC npc in registeredNPCs) {
                        DrawButton(npc);
                    }
                }
            }
        }
        using (new EditorGUILayout.VerticalScope()) {
            EditorUtils.WindowBoxLabel(selectedNPC == null ? "No NPC Selected" : selectedNPC.gameObject.name, UIStyles.CenteredLabelBold, GUILayout.Height(18));
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope(GUILayout.MinWidth(200))) {
                    EditorUtils.WindowBoxLabel("NPC Routine", UIStyles.CenteredLabelBold);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.textArea)) {
                        if (selectedNPC == null) {
                            EditorUtils.DrawScopeCenteredText("Select an NPC from the List;\n" +
                                                              "Its routine will appear here;");
                        } else {
                            using (new EditorGUILayout.ScrollViewScope(Vector2.zero, UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                                foreach (EntityAction action in selectedNPC.Routine) {
                                    DrawButton(action);
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
                            if (GUILayout.Button("Add Interact Action")) {
                                selectedNPC.Routine.Add(new ActionInteract());
                            }
                            if (GUILayout.Button("Add Look At Action")) {
                                selectedNPC.Routine.Add(new ActionLookAt());
                            }
                        }
                    }
                }
            }
        }
    }

    private void DrawButton(DummyNPC npc) {
        float width = EditorUtils.MeasureTextWidth(npc.gameObject.name, GUI.skin.font);
        if (GUILayout.Button(npc.gameObject.name, selectedNPC == npc
                                                  ? new GUIStyle(EditorStyles.numberField) {
                                                      alignment = TextAnchor.MiddleCenter,
                                                      normal = { textColor = UIColors.Blue } }
                                                  : GUI.skin.button)) {
            selectedAction = null;
            selectedNPC = npc;
        }
    }

    private void DrawButton(EntityAction action) {
        if (GUILayout.Button(action.GetType().Name, selectedAction == action
                                          ? new GUIStyle(EditorStyles.numberField) {
                                              alignment = TextAnchor.MiddleCenter,
                                              normal = { textColor = UIColors.Blue } }
                                          : GUI.skin.button)) {
            selectedAction = action;
        }
    }
}