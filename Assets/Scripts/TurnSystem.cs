using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
    [SerializeField] private TurnDisplay turnDisplay;
    // This list contains all entities AND the order in which they take their turn
    public List<Entity> turnOrder = new List<Entity>();
    public Entity activeEntity;
    private int _activeEntityIdx;

    void Awake ()
    {
        InitializeSingleton();
        _activeEntityIdx = -1;
    }
    
    void Start() {
        // Initializes all entities in scene
        PopulateEntitiesAndTurnOrder();
        turnDisplay.InitializeEntitiesTurnDisplays(turnOrder);
        // Begin new turn
        if (turnOrder.Count > 0)
        {
            SwitchTurn();
        }
        UpdateInputController();
        ActiveEntityAction();
    }

    public void SwitchTurn() {
        _activeEntityIdx = (_activeEntityIdx + 1) % turnOrder.Count;
        activeEntity = turnOrder[_activeEntityIdx];
        turnDisplay.UpdateDisplays(turnOrder, _activeEntityIdx);
        UpdateInputController();
        ActiveEntityAction();
    }

    public void HandlePlayerTurn() {
        if (activeEntity is Monster && !activeEntity.IsMoving) {
            SwitchTurn();
        }
    }

    private void ActiveEntityAction() {
        if (activeEntity is Human) {
            ((Human)activeEntity).PerformAction();
        }
    }

    private void UpdateInputController() {
        InputController.Instance.enabled = IsPlayersTurn();
        turnDisplay.UpdateEndTurnButton(IsPlayersTurn());
    }

    public bool IsPlayersTurn()
    {
        return activeEntity is Monster;
    }

    void PopulateEntitiesAndTurnOrder()
    {
        List<Entity> entitiesFound = FindObjectsOfType<Entity>().ToList();
        
        // Sorts list so that Monster is always first
        int monsterIndex = -1;
        for (int i = 0; i < entitiesFound.Count; i++)
        {
            if (entitiesFound[i] as Monster != null)
            {
                monsterIndex = i;
                break;
            }
        }
        
        // If found monster, swap index 0 with monster's index
        if (monsterIndex > -1)
        {
            (entitiesFound[0], entitiesFound[monsterIndex]) = (entitiesFound[monsterIndex], entitiesFound[0]);
        }
        else
        {
            Debug.LogError("Monster not found in scene!!!");
        }

        turnOrder = entitiesFound;
    }
}
