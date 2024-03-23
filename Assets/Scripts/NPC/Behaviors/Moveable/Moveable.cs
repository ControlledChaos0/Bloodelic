using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class Moveable : Behavior
{
    private Monster _monster;
    public Monster Monster {
        get => _monster;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetRoutine(new MoveableRoutine());
        name = "Move";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool CheckValid()
    {
        return GetComponent<Entity>().Movement > 0;
    }
}
