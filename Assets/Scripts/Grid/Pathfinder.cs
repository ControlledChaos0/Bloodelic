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
    public static GridPath FindPath(GridCell start, GridCell target) {
        if (start == null || target == null) {
            throw new ArgumentNullException("GridCell Arguments must not be null");
        }
        if (start.Equals(target)) {
            return new GridPath();
        }

        Dictionary<GridCell, bool> searched = new(GridManager.LevelGrid.Grid.Count);
        PriorityQueue<GridCell, float> toBeSearched = new();
        toBeSearched.Enqueue(start, 0);
        start.g = 0;

        bool end = false;

        while (toBeSearched.Count > 0) {
            GridCell current = toBeSearched.Dequeue();
            GridCell[] neighbors = current.Neighbors;

            foreach (GridCell cell in neighbors) {
                if (cell == null) {
                    continue;
                }

                bool value;
                if (cell.Equals(target)) {
                    cell.PathTo = current;
                    end = true;
                    break;
                } else if (!searched.ContainsKey(cell)) {

                }

                cell.PathTo = current;

                float g = current.g + Vector3.Distance(cell.Position.Position, current.Position.Position);
                float h = cell.FindHeuristic(target);
                float f = g + h;

                if (cell.F > f) {
                    
                }
            }

            if (end) {
                break;
            }

            searched.Add(current, true);
        }

        return new GridPath();
    }
}
