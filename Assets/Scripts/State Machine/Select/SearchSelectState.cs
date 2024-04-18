using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchSelectState : SelectState
{
    private GameObject _hoveredObject;
    private Selectable _hoveredSelectable;

    public override void EnterState()
    {
        if (stateMachine.Selectable != null) {
            stateMachine.Selectable.Deactivate();
            stateMachine.Selectable = null;
        }
        if (stateMachine.UIState != null) {
            stateMachine.CurrUI = null;
            stateMachine.CurrBehavCont = null;
            stateMachine.UIState = null;
        }
        Deactivate();
        Activate();
    }
    public override void UpdateState()
    {
        //dunno if it does anything
    }
    public override void ExitState()
    {
        stateMachine.Selectable = _hoveredSelectable;
        stateMachine.Selectable.Activate();
        Deactivate();
    }

    public void Activate() {
        CameraController.Instance.ClickAction += Select;
        CameraController.Instance.HoverAction += Hover;
        HoverClear();
    }
    public void Deactivate() {
        CameraController.Instance.ClickAction -= Select;
        CameraController.Instance.HoverAction -= Hover;
    }

    private void Select(GameObject gO) {
        if (_hoveredSelectable == null) {
            return;
        }
        stateMachine.ChangeState(new UISelectState());
    }
    private void Hover(GameObject gO) {
        //Debug.Log(gO);
        if (gO == null) {
            HoverClear();
            return;
        }
        if (gO.Equals(_hoveredObject)) {
            return;
        }
        //Debug.Log("HoverUnselected Running");
        Selectable selectable = GameObjectHelper.GetSelectableObject(gO);
        if (selectable == null) {
            HoverClear();
            return;
        }
        _hoveredObject = gO;
        _hoveredSelectable = selectable;
        _hoveredSelectable.HoverSelect();

        _hoveredSelectable.UIObject.SetActive(true);
        _hoveredSelectable.GetBehaviorController.CheckValid();
    }

    private void HoverClear() {
        if (_hoveredSelectable != null) {
            _hoveredSelectable.UIObject.SetActive(false);
            _hoveredSelectable.HoverDeselect();
        }
        _hoveredSelectable = null;
        _hoveredObject = null;
    }
}
