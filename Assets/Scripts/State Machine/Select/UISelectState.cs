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

    }
}
