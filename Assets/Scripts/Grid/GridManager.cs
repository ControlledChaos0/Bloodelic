using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class GridManager
{
    public static GameObject LevelGridGO
    {
        get => _levelGridGO;
        set => _levelGridGO = value;
    }
    public static LevelGrid LevelGrid
    {
        get => _levelGrid;
        set => _levelGrid = value;
    }
    private static GameObject _levelGridGO;
    private static LevelGrid _levelGrid;
    
    public static void SetLevelGrid(GameObject gameObject) {
        LevelGridGO = gameObject;
        LevelGrid = _levelGridGO.GetComponent<LevelGrid>();
    }
}
