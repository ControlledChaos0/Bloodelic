using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectState : SelectState
{
    private WorldUI _worldUI;
    public override void EnterState() {
        stateMachine.UIState = this;
        stateMachine.CurrUI.Activate();
    }
    public override void UpdateState() {

    }
    public override void ExitState() {
        stateMachine.CurrUI.Deactivate();
    }

    // public void Activate() {
    //     Debug.Log("Activate UI");
    //     gameObject.SetActive(true);
    //     AddButtons(ObjSelect.GetBehaviorController.PollBehaviors(_button));
    //     ObjSelect.ClickAction += Click;
    // }
    // public void Deactivate() {
    //     ObjSelect.GetBehaviorController?.DestroyButtons();
    //     ObjSelect.ClickAction -= Click;
    //     gameObject.SetActive(false);
    // }
}
