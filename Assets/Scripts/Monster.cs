using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class Monster : Entity
{
    [SerializeField]
    private PlayerTurnMachine _playerTurnMachine;
    private GridCell currPosCell;
    private GridPath currPosPath;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void SelectMonster(GameObject gameObject) {
        Debug.Log("SELECTED MONSTER!!!!!!!!!");
        if (!gameObject.Equals(this.gameObject)) {
            return;
        }
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
        currPosPath?.RevertColor();
        currPosPath = FindPath(gridCell);
        currPosPath.TurnBlue();
    }
    public void ChoosePath(GridCell gridCell) {
        ArgumentNullExceptionUse.ThrowIfNull(gridCell);
        _playerTurnMachine.ChangeState(_playerTurnMachine.idleState);
        Move(currPosPath);
    }
}
