using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable))]
public class BehaviorController : MonoBehaviour
{
    private Selectable _selectable;
    private List<Behavior> _behaviors;

    public Selectable Selectable {
        get => _selectable;
    }
    // Start is called before the first frame update
    void Start()
    {
        _selectable = GetComponent<Selectable>();
        //This is probably something to do before Start is even called, but good for now :)
        //Editor tools are a bitch :')
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBehavior(Behavior behavior) {
        if (_behaviors == null) {
            _behaviors = new();
        }
        _behaviors.Add(behavior);
    }
}
