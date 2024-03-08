using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    //Probably need to make a custom button script to handle all of this
    public List<GameObject> PollBehaviors(GameObject buttonObj) {
        List<GameObject> buttons = new();
        foreach (Behavior behavior in _behaviors) {
            if (behavior.CheckValid()) {
                GameObject buttonRealObj = Instantiate(buttonObj);
                buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(behavior.Name);
                Button button = buttonRealObj.GetComponent<Button>();
                behavior.BehaviorButton = button;
                button.onClick.AddListener(behavior.StartBehaviorAction);
                buttons.Add(buttonRealObj);
            }
        }
        return buttons;
    }
    public void DestroyButtons() {
        foreach (Behavior behavior in _behaviors) {
            Destroy(behavior.BehaviorButton?.gameObject);
        }
    }
}
