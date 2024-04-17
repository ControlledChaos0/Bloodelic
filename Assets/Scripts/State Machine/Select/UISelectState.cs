using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectState : SelectState
{
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
        Debug.Log(stateMachine.CurrBehavCont.Behaviors.Count);
        stateMachine.CurrUI.AddButtons(stateMachine.CurrBehavCont.Behaviors);
        CameraController.Instance.ClickAction += Click;
    }
    public void Deactivate() {
        stateMachine.CurrUI.DestroyButtons(stateMachine.CurrBehavCont.Behaviors);
        CameraController.Instance.ClickAction -= Click;
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
