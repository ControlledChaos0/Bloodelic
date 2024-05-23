using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using UnityEngine;

// basic human class that does the PerformAction
[RequireComponent(typeof(Killable))]
public class Human : DummyNPC
{
    [SerializeField]
    private Killable _killable;
    [SerializeField]
    private RagdollSwitch _ragdollSwitch;

    public RagdollSwitch RagdollSwitch {
        get { return _ragdollSwitch;}
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _killable.Human = this;
        base.Start();
        //HumanManager.Instance.ClickAction += Select;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void PostGridMovement()
    {
        // If human occupies an exit cell
        if (gridCellMapExitsDictionary.ContainsKey(occupiedCell))
        {
            MapExit exitUsed = gridCellMapExitsDictionary[occupiedCell];
            //Debug.Log(name + " escaped using Exit at " + occupiedCell.name);
            // Game Over...
            
            OccupiedCell.Unoccupy(this);
            StopAllCoroutines();
            gameObject.SetActive(false);
            TurnSystem.Instance.RemoveEntity(this);
            TurnSystem.Instance.SwitchTurn();
        }

        // Update AI responses
        if (aiBrain != null)
        {
            aiBrain.UpdateResponses(aiBrain.currentState);
        }
    }

    private void OnEnable() {
        // if (HumanManager.Instance != null) {
        //     HumanManager.Instance.ClickAction -= Select;
        //     HumanManager.Instance.ClickAction += Select;
        // }
    }
    private void OnDisable() {
        // HumanManager.Instance.ClickAction -= Select;
    }

    public void PerformAction() {
        if (aiBrain)
        {
            StartCoroutine(currentCoroutine = aiBrain.PerformAICoroutine());
        }
        // Just do default behavior if no AI Brain component is assigned to NPC
        else
        {
            StartCoroutine(DefaultBehavior());
        }
    }

    IEnumerator DummyTurnBehavior() {
        //Debug.Log("human turn! :D");
        yield return new WaitForSeconds(2.0f);
        TurnSystem.Instance.SwitchTurn();
    }
    
    // @todo: move to AI class
    IEnumerator DefaultBehavior()
    {
        EntityAction action = GetNextActionInRoutine();
        ActionMove moveAction = action as ActionMove;
        // This action is a Move Action => have NPC move to the destination tile
        if (moveAction != null)
        {
            
            yield return StartCoroutine(MoveCoroutine(moveAction.targetCell));
        }
        yield return new WaitForSeconds(0.25f);
        TurnSystem.Instance.SwitchTurn();
    }

    public void Die() {
        aiBrain.enabled = false;
        //Debug.Log(entityLOS);
        //Debug.Log(selectable);
        selectable.HoverAction -= entityLOS.ShowSight;
        selectable.UnhoverAction -= entityLOS.HideSight;
        entityLOS.ClearTiles();

        _ragdollSwitch.TurnOnRagdoll();

        entityLOS.enabled = false;
    }
}
