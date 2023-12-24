using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGrid))]
[CanEditMultipleObjects]
public class LevelGridEditor : Editor
{
    private Transform _gridTransform;
    private GameObject _corner1;
    private GameObject _corner2;

    private SerializedProperty _cornerProperty1;
    private SerializedProperty _cornerProperty2;
    private SerializedProperty _gridSize;
    private SerializedProperty _gridPrefab;
    private bool _corners;
    private bool _gridProps;
    private bool _create;
    //private Rect rect = new(0, 0, EditorGUIUtility.currentViewWidth, EditorGUIUtility.standardVerticalSpacing);

    private void OnEnable() {
        _gridTransform = (target as LevelGrid).gameObject.transform;
        _corner1 = _gridTransform.GetChild(0).gameObject;
        _corner2 = _gridTransform.GetChild(1).gameObject;

        _cornerProperty1 = serializedObject.FindProperty("corner1");
        _cornerProperty2 = serializedObject.FindProperty("corner2");
        _gridSize = serializedObject.FindProperty("gridSpaceSize");
        _gridPrefab = serializedObject.FindProperty("gridCellPrefab");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        _create = (target as LevelGrid).Grid.Count == 0 && _corner1.activeSelf;
        if (_create) {
            _corners = EditorGUILayout.BeginFoldoutHeaderGroup(_corners, "Corners");
            if (_corners) {
                EditorGUILayout.PropertyField(_cornerProperty1);
                EditorGUILayout.PropertyField(_cornerProperty2);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            _gridProps = EditorGUILayout.BeginFoldoutHeaderGroup(_gridProps, "Grid Properties");
            if (_gridProps) {
                EditorGUILayout.PropertyField(_gridSize);
                EditorGUILayout.PropertyField(_gridPrefab);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (GUILayout.Button("Create Level Grid")) {                
                (target as LevelGrid).CallCreateGrid();
            }
        } else {
            if (GUILayout.Button("Delete Level Grid")) {
                (target as LevelGrid).ClearGrid();
                while (_gridTransform.childCount > 2) {
                    DestroyImmediate(_gridTransform.GetChild(2).gameObject);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
