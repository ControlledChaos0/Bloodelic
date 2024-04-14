using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : Singleton<HumanManager>
{
    public event Action<Human> ClickAction;

    private void Awake()
    {
        InitializeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {
        //CameraController.Instance.ClickAction += OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        //CameraController.Instance.ClickAction -= OnClick;
    }

    private void OnClick(GameObject gameObject)
    {
        Human human = gameObject.GetComponent<Human>();
        if (human != null)
        {
            ClickAction?.Invoke(human);
        }
        else
        {
            ClickAction?.Invoke(null);
        }
    }
}
