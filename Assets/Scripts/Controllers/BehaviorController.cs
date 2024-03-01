using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable))]
public class BehaviorController : MonoBehaviour
{
    private List<Behavior> behaviors;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBehavior(Behavior behavior) {
        behaviors.Add(behavior);
    }
}
