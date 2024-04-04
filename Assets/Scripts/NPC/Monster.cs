using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor;
using UnityEditor.Recorder.Input;
using UnityEngine;

[RequireComponent(typeof(Moveable))]
public class Monster : Entity
{
    [Header("Behaviors")]
    [SerializeField]
    private Moveable _moveable;
    private GridCell currPosCell;
    private GridPath currPosPath;
    private Transform _lookAt;

    public int LengthOfPath {
        get => currPosPath.Count - 1;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        _moveable.Monster = this;
        base.Start(); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        StartMonster();
    }

    private void StartMonster() {
        _moveable.Monster = this;
    }

    public void ShowPath(GridCell gridCell) {
        ArgumentNullExceptionUse.ThrowIfNull(gridCell);
        
        if (gridCell.Equals(currPosCell)) {
            return;
        }
        
        // Find next path and store to temporary
        GridPath nextPath = FindPathWithMoveLimit(gridCell);
        // Skip here so we maintain visuals of the previous path
        if (nextPath == null)
        {
            return;
        }    
        
        //currPosPath?.HidePath();
        currPosPath?.RevertColor();
        currPosPath = nextPath;
        currPosPath.TurnBlue();
        //currPosPath.ShowPath();
    }
    public void ChoosePath(GridCell gridCell) {
        ArgumentNullExceptionUse.ThrowIfNull(gridCell);
        foreach (GridCell cell in currPosPath.Path) {
            Debug.Log(cell);
        }
        Move(currPosPath);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
    }

    #endif
}
