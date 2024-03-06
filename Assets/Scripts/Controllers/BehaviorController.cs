using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable))]
public class BehaviorController : MonoBehaviour
{
    private Selectable _selectable;
    private GameObject _uiObject;
    private WorldUI _uiScript;
    private List<Behavior> _behaviors;

    public Selectable Selectable {
        get => _selectable;
    }
    // Start is called before the first frame update
    void Start()
    {
        _selectable = GetComponent<Selectable>();
        _uiObject = _selectable.UIObject;
        _uiScript = _uiObject.GetComponent<WorldUI>();
        //This is probably something to do before Start is even called, but good for now :)
        //Editor tools are a bitch :')
        if (_behaviors == null) {
            return;
        }
        for (int i = 0; i < _behaviors.Count; i++) {
            Behavior temp = _behaviors[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Deactivate() {
        
    }

    public void AddBehavior(Behavior behavior) {
        if (_behaviors == null) {
            _behaviors = new();
        }
        _behaviors.Add(behavior);
    }
}
