using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCManager))]
public class NPCManagerEditor : Editor {
    public override void OnInspectorGUI() {
        NPCManager npcManager = target as NPCManager;
        if (GUILayout.Button("Launch NPC Routine Tool")) {
            RoutineToolGUI.LaunchRoutineTool(npcManager);
        }
    }
}
