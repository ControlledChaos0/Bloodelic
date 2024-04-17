using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnthonysMathHelpers;
using Unity.Collections;
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

    [ReadOnly] public Monster monster;
    [ReadOnly] public MapExit nearestExit;
    
    // Default parameters
    public float monsterDetectionRange = 2.5f;
    public int cellsAroundMonsterWeight = 100;
    public float cellsAroundMonsterMaxDistance = 3f;
    
    #endregion
    
    void Start()
    {
        npc = GetComponent<DummyNPC>();
        monster = FindObjectOfType<Monster>();
        
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
                // Look for nearest exit
                FindNearestExit();
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
        if (npc == null)
        {
            return;
        }
        
        switch (state)
        {
            // Determines next action in routine
            case AIState.Default:
                npc.NextAction = npc.GetNextActionInRoutine();
                break;
            case AIState.Investigative:

                break;
            case AIState.Distressed:
                FindAllMovableCellsTowardsNearestExit();
                break;
        }
    }

    public void UpdateResponses(AIState state)
    {
        switch (state)
        {
            case AIState.Default:
                // Check if ai can detect monster, use radius for now
                if (monster != null)
                {
                    if (Vector3.Distance(monster.OccupiedCell.Position.Position, npc.OccupiedCell.Position.Position) <= monsterDetectionRange)
                    {
                        npc.StopAllCoroutines();   
                        TurnSystem.Instance.SwitchTurn();
                        ChangeState(AIState.Distressed);
                    }
                }
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
                yield return StartCoroutine(DistressedStateBehavior());;
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
            npc.Animator.SetTrigger("TrHumanWalk");
            yield return StartCoroutine(npc.MoveCoroutine(moveAction.targetCell));
        }
        npc.Animator.ResetTrigger("TrHumanWalk");
        npc.Animator.SetTrigger("TrHumanDance");
        yield return new WaitForSeconds(0.25f);
        TurnSystem.Instance.SwitchTurn();
    }

    IEnumerator DistressedStateBehavior()
    {
        if (nearestExit != null)
        {
            // Find cell to move to if there are movable options
            if (movableCellsTowardsExit.Count > 0)
            {
                // Check if there are duplicates
                List<GridCellAndScore> temp = new List<GridCellAndScore>();
                float minDistToExit = Mathf.Infinity;
                foreach (var c in movableCellsTowardsExit)
                {
                    // If a new minDist is found, clears list and add cell
                    if (c.score < minDistToExit)
                    {
                        minDistToExit = c.score;
                        temp = new List<GridCellAndScore>();
                        temp.Add(c);
                    }
                    // If equal distance, add to list
                    else if (c.score == minDistToExit)
                    {
                        temp.Add(c);
                    }
                }

                GridCell desiredCell = null;
                if (temp.Count > 0)
                {
                    temp.Shuffle();
                    desiredCell = temp[0].cell;
                }

                if (desiredCell != null)
                {
                    yield return StartCoroutine(npc.MoveCoroutine(desiredCell));
                }
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        TurnSystem.Instance.SwitchTurn();
    }
    
#endregion
    
#region Distressed
    public struct GridCellAndScore
    {
        public GridCell cell;
        public float score;

        public GridCellAndScore(GridCell cell, float score)
        {
            this.cell = cell;
            this.score = score;
        }
    }

    List<GridCellAndScore> movableCellsTowardsExit = new List<GridCellAndScore>();
    
    
    
    // Iterate through all map exits, calculate the movement distances towards them (pathfinding)
    //  and select the one with the shortest path
    void FindNearestExit()
    {
        float minDistance = Mathf.Infinity;
        MapExit foundExit = null;
        
        foreach (var e in npc.mapExits)
        {
            Pathfinder.moveLimit = Mathf.Infinity;
            GridPath path =  Pathfinder.FindPath(npc.OccupiedCell, e.cell);
            // No valid path
            if (path == null)
            {
                continue;
            }
            // Calculate total distance of this path
            float totalDistance = 0f;
            GridCell currCell = null;
            while (path.Count != 0)
            {
                currCell = path.PopFront();
                totalDistance += currCell.F;
            }

            if (totalDistance < minDistance)
            {
                minDistance = totalDistance;
                foundExit = e;
            }
        }

        nearestExit = foundExit;
    }

    void FindAllMovableCellsTowardsNearestExit()
    {
        if (nearestExit != null)
        {
            // Pathfind and move towards exit based on NPC's movement stat
            Vector3 dirToExit = (nearestExit.transform.position - transform.position).normalized;
            dirToExit.y = 0; // only care about 2D grid movement for now

            List<GridCellAndScore> movableCellsByDistance = new List<GridCellAndScore>();
            // Get all tiles that this AI can move onto
            foreach (var cell in LevelGrid.Instance.allGridCells)
            {
                // Skip all wall cells
                if (cell.IsWall()) { continue; }
                // Skip occupied cells
                if (cell.IsOccupied()) { continue; }
                // Skip cells that are outside movement range
                Vector3 posToCell = (cell.transform.position - this.transform.position);
                if (posToCell.magnitude > npc.Movement) { continue; }
                // Skip cells with higher elevations for now
                if (posToCell.y != 0) { continue;}
                
                // // Skip cells that are not in the direction of movement (experimental)
                // Vector3 dirToCell = posToCell.normalized;
                // if (Vector3.Dot(dirToCell, dirToExit) < -0.5f) { continue; }
                
                GridCellAndScore validCell = new GridCellAndScore(cell, Vector3.Distance(cell.Position.Position, nearestExit.cell.Position.Position));
                // If cells are close to monster, add penalty scores so AI avoids them
                //   fall off based on distance to monster
                float distFromCellToMonster = Vector3.Distance(cell.transform.position, monster.transform.position);
                if (distFromCellToMonster <= cellsAroundMonsterMaxDistance)
                {
                    float nearMonsterScore = Mathf.Lerp(cellsAroundMonsterWeight, 0,
                        distFromCellToMonster / cellsAroundMonsterMaxDistance);
                    validCell.score += nearMonsterScore;
                }
                // Add valid cell
                movableCellsByDistance.Add(validCell);
            }

            movableCellsTowardsExit = movableCellsByDistance;
            
            // Sort by minimum score and pick the best cell
            movableCellsTowardsExit.OrderBy(c => c.score);
        }
        else
        {
            movableCellsTowardsExit = new List<GridCellAndScore>();
        }
    }
    
#endregion

#region Debug

    public bool debugMode = true;

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!debugMode) return;
        
        Color debugColor = Color.white;
        switch (currentState)
        {
            case AIState.Investigative:
                debugColor = Color.yellow;
                break;
            case AIState.Distressed:
                debugColor = Color.red;
                // Draw path towards exit tile
                if (nearestExit != null)
                {
                    Handles.color = debugColor;
                    Handles.DrawAAPolyLine(transform.position, nearestExit.transform.position);
                    Handles.SphereHandleCap(0,  nearestExit.transform.position, Quaternion.identity, 0.15f, EventType.Repaint);
                }
                // Draw all moveable cells
                debugColor = Color.magenta;
                foreach (var c in movableCellsTowardsExit)
                {
                    Handles.color = debugColor;
                    GUIStyle style = new GUIStyle();
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = debugColor;
                    Handles.DrawAAPolyLine(transform.position, c.cell.transform.position);
                    String cellScoreString = string.Format("{0:0}", c.score);
                    Handles.Label(c.cell.transform.position + Vector3.up, cellScoreString, style);
                }
                debugColor = Color.red;
                break;
        }

        Handles.color = debugColor;
        string debugString = string.Format("AI State: {0}", currentState);
        if (currentState == AIState.Distressed)
        {
            debugString += " !!! ";
        }
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 16;
        textStyle.normal.textColor = debugColor;
        Handles.Label(transform.position + Vector3.up * 2f, debugString, textStyle);
        
        // Action debug
        if (currentState == AIState.Default)
        {
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

                textStyle.fontSize = 12;
                textStyle.normal.textColor = debugColor;
                debugString = string.Format("Next Action: {0}", npc.NextAction.ToString());
                Handles.Label(transform.position + Vector3.up * 1.35f, debugString, textStyle);
            }  
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