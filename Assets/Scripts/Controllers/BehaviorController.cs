using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class BehaviorController : MonoBehaviour
{
    private List<Behavior> _behaviors;
    public List<Behavior> Behaviors {
        get {
            _behaviors ??= new();
            return _behaviors;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //This is probably something to do before Start is even called, but good for now :)
        //Editor tools are a bitch :')
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeBehaviors() {
        _behaviors = new();
        Behavior[] arr = GetComponents<Behavior>();
        Debug.Log($"Length of Behavior Array: {arr.Length}");
        foreach (Behavior item in arr)
        {
            item.InitializeBehavior();
            _behaviors.Add(item);
            Debug.Log(item);
        }
    }

    public void ResetBehaviors() {
        foreach (Behavior behavior in Behaviors) {
            behavior.ResetBehavior();
        }
    }
    
}
