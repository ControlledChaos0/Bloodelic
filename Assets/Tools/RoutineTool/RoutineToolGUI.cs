using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoutineToolGUI : EditorWindow {

    public static RoutineToolGUI mainWindow;

    public static void LaunchRoutineTool(NPCManager npcManager) {
        mainWindow = GetWindow<RoutineToolGUI>("NPC Routine Tool");
        mainWindow.Init(npcManager);
    }

    private NPCManager npcManager;
    private DummyNPC[] registeredNPCs;

    private void Init(NPCManager npcManager) {
        this.npcManager = npcManager;
        registeredNPCs = npcManager.LoadEntities<DummyNPC>();
    }

    void OnGUI() {

    }
}