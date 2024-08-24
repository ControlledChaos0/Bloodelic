using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIButtonPool : Singleton<WorldUIButtonPool>
{
    [SerializeField] private GameObject _button;
    [SerializeField] private const int ButtonPoolNum = 5;

    private GameObject[] _buttons;
    private bool[] _buttonsActive;

    private void Awake()
    {
        InitializeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_button)
        {
            throw new NullReferenceException("Button Prefab not assigned in WorldUIButtonPool");
        }

        _buttons = new GameObject[ButtonPoolNum];
        _buttonsActive = new bool[ButtonPoolNum];

        for (int i = 0; i < ButtonPoolNum; i++)
        {
            _buttons[i] = Instantiate(_button, transform);
            _buttons[i].SetActive(false);
            _buttonsActive[i] = false;
        }
    }

    public GameObject GetButton()
    {
        for (int i = 0; i < _buttonsActive.Length; i++)
        {
            if (!_buttonsActive[i])
            {
                _buttons[i].SetActive(true);
                _buttonsActive[i] = true;
                return _buttons[i];
            }
        }

        throw new OverflowException("WorldUIButtonPool::GetButton failed. All buttons in WorldUIButtonPool active.");
    }

    public void ResetButton(GameObject button)
    {
        if (!button)
        {
            throw new NullReferenceException("Button passed into WorldUIButtonPool::ResetButton null");
        }

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (button.Equals(_buttons[i]))
            {
                _buttons[i].transform.SetParent(transform);
                _buttons[i].SetActive(false);
                _buttonsActive[i] = false;
                break;
            }
        }
    }
}
