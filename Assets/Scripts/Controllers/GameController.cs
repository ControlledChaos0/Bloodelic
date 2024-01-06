using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static readonly object padlock = new();
    private static GameController _instance;
    private Monster _monster;

    public static GameController Instance {
        get {
            lock (padlock) {
                if (_instance == null) {
                    _instance = new();
                }
                return _instance;
            }
        }
    }

    private void Awake() {
        
    }
    private void Start() {
        
    }
    private void Update() {
        
    }
    private void OnEnable() {
        
    }
    private void OnDisable() {
        Clear();
    }

    public void Clear() {
        _instance = new();
    }

    public void PlayerTurn() {

    }
    public void SystemTurn() {

    }
    public void MovePlayer(GridPath path) {
        _monster.Move(path);
    }

    public void SetMonster(Monster monster) {
        _monster = monster;
    }
}
