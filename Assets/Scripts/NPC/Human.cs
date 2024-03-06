using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basic human class that does the PerformAction
public class Human : DummyNPC
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        base.Start();
        HumanManager.Instance.ClickAction += Select;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    private void OnEnable() {
        if (HumanManager.Instance != null) {
            HumanManager.Instance.ClickAction -= Select;
            HumanManager.Instance.ClickAction += Select;
        }
    }
    private void OnDisable() {
        HumanManager.Instance.ClickAction -= Select;
    }

    public void PerformAction() {
        if (aiBrain)
        {
            StartCoroutine(aiBrain.PerformAICoroutine());
        }
        // Just do default behavior if no AI Brain component is assigned to NPC
        else
        {
            StartCoroutine(DefaultBehavior());
        }
    }

    IEnumerator DummyTurnBehavior() {
        Debug.Log("human turn! :D");
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

    public void Select(Human gO)
    {

    }

    public void HoverSelect(Human gO)
    {
        
    }
}
