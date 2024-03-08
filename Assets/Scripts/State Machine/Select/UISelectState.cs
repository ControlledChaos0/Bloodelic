using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectState : SelectState
{
    public override void EnterState() {
        stateMachine.Selectable.UIScript.Activate();
    }
    public override void UpdateState() {

    }
    public override void ExitState() {

    }
}
