using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
    [SerializeField] private TurnDisplay turnDisplay;
    public Entity[] turnOrder;
    private Entity _activeEntity;
    private int _activeEntityIdx;

    void Awake ()
    {
        InitializeSingleton();
        _activeEntityIdx = 0;
        _activeEntity = turnOrder[0];
    }
    
    void Start() {
        turnDisplay.UpdateDisplays(turnOrder, _activeEntityIdx);
    }

    // Update is called once per frame
    void Update()
    {

   
    }
}
