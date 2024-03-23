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
        get => _behaviors;
    }

    // Start is called before the first frame update
    void Start()
    {
        _behaviors = new();
        Behavior[] arr = GetComponents<Behavior>();
        foreach (Behavior item in arr)
        {
            _behaviors.Add(item);
        }
        //This is probably something to do before Start is even called, but good for now :)
        //Editor tools are a bitch :')
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
