using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// This class controls how NPC AIs behave, state-machine based
/// </summary>
public class AIBrain : MonoBehaviour
{
    #region Properties

    public AIState currentState;
    DummyNPC npc { get; set; }
    
    #endregion
    
    void Start()
    {
        npc = GetComponent<DummyNPC>();
        // AIs start with Default state
        BeginState(AIState.Default);

        TurnSystem.Instance.OnNewTurn += OnNewTurn;
    }

    void OnDestroy()
    {
        TurnSystem.Instance.OnNewTurn -= OnNewTurn;
    }

    void BeginState(AIState state)
    {
        currentState = state;
        EnterState(currentState);
        UpdateState(currentState);
    }

    void ChangeState(AIState nextState)
    {
        // Exit state
        ExitState(currentState);
        
        // Set state
        currentState = nextState;

        // Enter state
        EnterState(currentState);
    }

    void EnterState(AIState state)
    {
        switch (state)
        {
            // Enable sensors
            case AIState.Default:

                break;
            case AIState.Investigative:

                break;
            // Disable sensors
            case AIState.Distressed:

                break;
        }
    }

    void ExitState(AIState state)
    {
        switch (state)
        {
            case AIState.Default:

                break;
            case AIState.Investigative:

                break;
            case AIState.Distressed:

                break;
        }
    }

    public void UpdateCurrentState()
    {
        UpdateState(currentState);
    }
    
    // This only really needs to update when events happen rather than per frame
    void UpdateState(AIState state)
    {
        switch (state)
        {
            // Determines next action in routine
            case AIState.Default:
                npc.NextAction = npc.GetNextActionInRoutine();
                break;
            case AIState.Investigative:

                break;
            case AIState.Distressed:

                break;
        }
    }

    void OnNewTurn()
    {
        UpdateState(currentState);
    }

#region Coroutines
    public IEnumerator PerformAICoroutine()
    {
        if (npc == null)
        {
            Debug.LogError("AI Brain has no reference to any NPC!!");
            yield break;
        }
        
        switch (currentState)
        {
            case AIState.Default:
                yield return StartCoroutine(DefaultStateBehavior());
                break;
            case AIState.Investigative:
                
                break;
            case AIState.Distressed:
                yield return null;
                break;
        }
    }

    // AI performs default routine in this state
    IEnumerator DefaultStateBehavior()
    {
        EntityAction action = npc.NextAction;
        // This action is a Move Action => have NPC move to the destination tile
        if (action is ActionMove)
        {
            ActionMove moveAction = (ActionMove)action;
            yield return StartCoroutine(npc.MoveCoroutine(moveAction.targetCell));
        }
        
        yield return new WaitForSeconds(0.25f);
        TurnSystem.Instance.SwitchTurn();
    }
    
#endregion
    
#region Debug
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color debugColor = Color.white;
        switch (currentState)
        {
            case AIState.Investigative:
                debugColor = Color.yellow;
                break;
            case AIState.Distressed:
                debugColor = Color.red;
                break;
        }

        Handles.color = debugColor;
        string debugString = string.Format("AI State: {0}", currentState);
        Handles.Label(transform.position + Vector3.up, debugString);
        
        // Action debug
        if (npc != null && npc.NextAction != null)
        {
            if (npc.NextAction is ActionMove)
            {
                debugColor = Color.cyan;
                Handles.color = debugColor;
                Handles.DrawAAPolyLine(transform.position, ((ActionMove)npc.NextAction).targetCell.transform.position);
                Handles.SphereHandleCap(0,  ((ActionMove)npc.NextAction).targetCell.transform.position, Quaternion.identity, 0.3f, EventType.Repaint);
            }
            else if (npc.NextAction is ActionInteract)
            {
                debugColor = new Color(1f, 0.8f, 0.5f);
            }
            else if (npc.NextAction is ActionWait)
            {
                debugColor = Color.gray;
            }

            debugString = string.Format("Next Action: {0}", npc.NextAction.ToString());
            Handles.Label(transform.position + Vector3.up * 0.75f, debugString);
        }
        
        Handles.color = Color.white;
    }
#endif
#endregion
}

public enum AIState
{
    Default,
    Investigative,
    Distressed,
}