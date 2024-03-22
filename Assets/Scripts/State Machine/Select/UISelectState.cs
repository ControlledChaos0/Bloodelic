using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectState : SelectState
{
    private WorldUI _worldUI;
    public override void EnterState() {
        stateMachine.UIState = this;
        Activate();
    }
    public override void UpdateState() {

    }
    public override void ExitState() {
        Deactivate();
    }

    public void Activate() {
        Debug.Log("Activate UI");
        stateMachine.CurrUI = stateMachine.Selectable.UIScript;
        stateMachine.CurrBehavCont = stateMachine.Selectable.GetBehaviorController;

        stateMachine.CurrUI.gameObject.SetActive(true);
        //AddButtons(ObjSelect.GetBehaviorController.PollBehaviors(_button));
        stateMachine.Selectable.ClickAction += Click;
    }
    public void Deactivate() {
        stateMachine.CurrBehavCont.DestroyButtons();
        stateMachine.Selectable.ClickAction -= Click;
        stateMachine.CurrUI.gameObject.SetActive(false);
    }

    public void Click(GameObject gO) {
        Debug.Log("Do you work :D");
        if (!stateMachine.CurrUI.CheckIfUIObject(gO)) {
            stateMachine.Selectable.Deactivate();
            stateMachine.ChangeState(stateMachine.SearchState);
        }
    }
}
