using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    public void EnterState(StateMachine sm);
    public void UpdateState(StateMachine sm);
    public void ExitState(StateMachine sm);
}
