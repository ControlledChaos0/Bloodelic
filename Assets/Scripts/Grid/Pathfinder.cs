using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C5;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

/*
    Code inspired by Matthew-J-Spencer's A* Pathfinding Tutorial and A* Pathfinding Project
    Matthew-J-Spencer: https://github.com/Matthew-J-Spencer/Pathfinding/tree/main
    A* Pathfinding Project: https://arongranberg.com/astar/front
*/
public class Pathfinder
{
    // Additional variables to adjust pathfinding for entities
    //   these are all static and need to be set/reset everytime Pathfinder is utilized
    #region Additional Properties
    public static float moveLimit = Mathf.Infinity;
    public static Entity entity;
    #endregion

    public static List<GridCell> ActivateCells(GridCell start, int distance) {
        List<GridCell> list = new();
        ActivateCellsHelper(start, distance, 0, list);
        return list;
    }

    public static void ActivateCellsHelper(GridCell curr, int distance, int currDistance, List<GridCell> gridCells) {
        if (currDistance < distance) {
            GridCell[] neighbors = curr.Neighbors;
            foreach (GridCell cell in neighbors) {
                gridCells.Add(cell);
                ActivateCellsHelper(cell, distance, currDistance + 1, gridCells);
                if (cell.IsShowing()) {
                     continue;
                }
                UnityEngine.Debug.Log($"{cell}: Distance is {currDistance}");
                cell.ShowCell();
            }
        }
    }
    
    public static GridPath FindPath(GridCell start, GridCell target, bool allowsNullIfNoneFound = false) 
    {
        if (start == null || target == null) {
            throw new ArgumentNullException("GridCell Arguments must not be null");
        }
        if (start.Equals(target)) {
            return new GridPath();
        }

        Dictionary<GridCell, bool> searched = new(LevelGrid.Instance.Grid.Count);
        Dictionary<GridCell, bool> toBeSearchedDic = new(LevelGrid.Instance.Grid.Count);
        PriorityQueue<GridCell, float> toBeSearched = new();
        toBeSearched.Enqueue(start, 0);
        toBeSearchedDic.Add(start, true);
        start.G = 0;
        start.PathTo = null;

        bool end = false;

        while (toBeSearched.Count > 0) {
            GridCell current = toBeSearched.Dequeue();
            if (searched.ContainsKey(current)) {
                continue;
            }
            GridCell[] neighbors = current.Neighbors;

            foreach (GridCell cell in neighbors) {
                if (cell == null) {
                    continue;
                }
                // If cell is occupied
                if (cell.IsOccupied() && cell.Occupant.BlockCells)
                {
                    continue;
                }
                bool value;
                if (searched.ContainsKey(cell)) {
                    continue;
                } else if (cell.Equals(target)) {
                    cell.PathTo = current;
                    end = true;
                    break;
                }

                float g = current.G + Vector3.Distance(cell.Position.Position, current.Position.Position);
                float h = cell.FindHeuristic(target);
                float f = g + h;

                // If distance to cell is above move limit
                if (f >= moveLimit)
                {
                    continue;
                }
                
                if (toBeSearchedDic.TryGetValue(cell, out value) && value && cell.F <= f) {
                    continue;
                }
                cell.SetF(g, h);
                cell.PathTo = current;
                toBeSearched.Enqueue(cell, f);
            }

            if (end) {
                return new GridPath(target);
            }

            searched.Add(current, true);
        }
        UnityEngine.Debug.Log("No path found");
        
        if (allowsNullIfNoneFound)
        {
            return null;
        }
        
        return new GridPath();
    }
}
