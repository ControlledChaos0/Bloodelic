using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Break))]
public class BreakEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
            SerializedProperty canBreakOnImpact = serializedObject.FindProperty("canBreakOnImpact");
            canBreakOnImpact.boolValue = EditorGUILayout.Toggle("Break On Impact", canBreakOnImpact.boolValue);
            EditorGUI.BeginDisabledGroup(!canBreakOnImpact.boolValue);
                SerializedProperty minBreakVelocity = serializedObject.FindProperty("minBreakVelocity");
                minBreakVelocity.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Break Velocity", minBreakVelocity.floatValue));
            EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        SerializedProperty breakBoostForce = serializedObject.FindProperty("breakBoostForce");
        breakBoostForce.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Break Force", breakBoostForce.floatValue));

        EditorGUILayout.BeginHorizontal();
            SerializedProperty destroyBottleAfterBreak = serializedObject.FindProperty("destroyBottleAfterBreak");
            destroyBottleAfterBreak.boolValue = EditorGUILayout.Toggle("Destroy Bottle", destroyBottleAfterBreak.boolValue);
            EditorGUI.BeginDisabledGroup(!destroyBottleAfterBreak.boolValue);
                EditorGUILayout.BeginVertical();
                    SerializedProperty destroyDelaySeconds = serializedObject.FindProperty("destroyDelaySeconds");
                    destroyDelaySeconds.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Delay", destroyDelaySeconds.floatValue));
                    SerializedProperty fadeDurationSeconds = serializedObject.FindProperty("fadeDurationSeconds");
                    fadeDurationSeconds.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Fade Duration", fadeDurationSeconds.floatValue));
                EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Break"))
        {
            Break breakScript = (Break)target;
            breakScript.Trigger();
        }
        EditorGUI.EndDisabledGroup();
    }
}
