using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    public void EnterState();
    public void UpdateState();
    public void ExitState();
}
