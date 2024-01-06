using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridCellPosition
{
    public GridCellPositionEnum PositionE {
        get => positionE;
        set => positionE = value;
    }
    public Vector3 Position{
        get => position;
        set => position = value;
    }
    [SerializeField]
    private GridCellPositionEnum positionE;
    [SerializeField]
    private Vector3 position;

    public GridCellPosition() : this(Vector3.up, GridCellPositionEnum.BOTTOM) {}
    public GridCellPosition(Vector3 position, GridCellPositionEnum positionE) {
        this.position = position;
        this.positionE = positionE;
    }

    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        // TODO: write your implementation of Equals() here
        return Position.Equals(((GridCellPosition)obj).Position) && PositionE == ((GridCellPosition)obj).PositionE;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        int hash = 5;
        hash += 89 * hash + (Position != null ? Position.GetHashCode() : 0);
        hash += 89 * hash + PositionE.GetHashCode();
        return hash;
    }

    public override string ToString()
    {
        return $"{position} {positionE}";
    }
}
