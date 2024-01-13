using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerMove : State
{
    private GridCell _gridCell;
    private GridPath _gridPath;
    private Monster _monster = Monster.MInstance;
    private CameraController _camera = new();

    public override void EnterState() {
        _camera.HitMask = 1 << 3;
    }
    public override void UpdateState() {
        RaycastHit hit = _camera.ClosestHit;
        if (hit.Equals(new RaycastHit())) {
            return;
        }
        GridCell cell = hit.transform.gameObject.GetComponent<GridCell>();
        if (_gridCell != null && cell.Equals(_gridCell)) {
            return;
        }
        _gridCell = cell;
        _gridPath.RevertColor();
        _gridPath = _monster.FindPath(_gridCell);
        _gridPath.TurnBlue();
    }
    public override void ExitState() {
        _camera.HitMask = 0;
    }
}
