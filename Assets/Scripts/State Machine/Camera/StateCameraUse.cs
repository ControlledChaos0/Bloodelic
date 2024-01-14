using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCameraUse : State
{
    public override void EnterState(StateMachine sm)
    {
        CameraMachine cm = sm as CameraMachine;
        InputController.Instance.MiddleHold += cm.cameraController.StartPanCamera;
        InputController.Instance.MiddleCancel += cm.cameraController.StopPanCamera;
        InputController.Instance.RightHold += cm.cameraController.StartRotateCamera;
        InputController.Instance.RightCancel += cm.cameraController.StopRotateCamera;
        InputController.Instance.LeftClick += cm.cameraController.ScreenClick;
        InputController.Instance.Scroll += cm.cameraController.ZoomCamera;
        InputController.Instance.Hover += cm.cameraController.Hover;
        cm.cameraController.HitMask = ConstantValues.EntityMask;
    }
    public override void UpdateState(StateMachine sm)
    {

    }
    public override void ExitState(StateMachine sm)
    {
        CameraMachine cm = sm as CameraMachine;
        InputController.Instance.MiddleHold -= cm.cameraController.StartPanCamera;
        InputController.Instance.MiddleCancel -= cm.cameraController.StopPanCamera;
        InputController.Instance.RightHold -= cm.cameraController.StartRotateCamera;
        InputController.Instance.RightCancel -= cm.cameraController.StopRotateCamera;
        InputController.Instance.LeftClick -= cm.cameraController.ScreenClick;
        InputController.Instance.Scroll -= cm.cameraController.ZoomCamera;
        InputController.Instance.Hover -= cm.cameraController.Hover;
    }
}
