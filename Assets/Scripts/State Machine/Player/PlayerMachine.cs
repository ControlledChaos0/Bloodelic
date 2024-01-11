using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is for controlling interactions when the player is choosing what the monster is doing on their turn
public class PlayerMachine : StateMachine
{
    private StatePlayerMove _moveState = new();
    private StatePlayerIdle _idleState = new();


    
    // Start is called before the first frame update
    protected override void Start()
    {
        ChangeState(_moveState);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
