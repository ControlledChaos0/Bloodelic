using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField]
    private GameObject objectInThisGridSpace = null;

    public GridCellPositionEnum PositionE {
        get;
        set;
    }
    public GridCellPosition Position {
        get;
        set;
    }
    private bool IsOccupied {
        get;
        set;
    }

    private int posX;
    private int posY;

    // Start is called before the first frame update
    void Start()
    {
        IsOccupied = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2Int GetPosition() {
        return new Vector2Int(posX, posY);
    }

    public void SetPosition(int x, int y) {
        posX = x;
        posY = y;
    }
}
