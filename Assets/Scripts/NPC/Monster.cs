using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class Monster : Entity
{
    [SerializeField]
    private PlayerTurnMachine _playerTurnMachine;
    private GridCell currPosCell;
    private GridPath currPosPath;
    private Transform _lookAt;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        selectable.SelectionAction += Select;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Select() {
        Debug.Log("SELECTED MONSTER!!!!!!!!!");
        CameraController.Instance.changePosition(gameObject.transform.position);
        Debug.Log("split");
        if (!_playerTurnMachine.moveState.canSwitch) {
            return;
        }        
        _playerTurnMachine.ChangeState(_playerTurnMachine.moveState);
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
        
        currPosPath?.HidePath();
        currPosPath?.RevertColor();
        currPosPath = nextPath;
        currPosPath.TurnBlue();
        currPosPath.ShowPath();
    }
    public void ChoosePath(GridCell gridCell) {
        ArgumentNullExceptionUse.ThrowIfNull(gridCell);
        _playerTurnMachine.ChangeState(_playerTurnMachine.idleState);
        Move(currPosPath);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string stateDebug = string.Format("Monster State: " + _playerTurnMachine.currentState);
        Handles.Label(transform.position + Vector3.up, stateDebug);
    }

    #endif
}
