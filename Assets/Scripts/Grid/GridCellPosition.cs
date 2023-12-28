using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridCellPosition
{
    public GridCellPositionEnum PositionE {
        get => _positionE;
        set => _positionE = value;
    }
    public Vector3 Position{
        get => _position;
        set => _position = value;
    }

    private GridCellPositionEnum _positionE;
    private Vector3 _position;

    public GridCellPosition() : this(Vector3.up, GridCellPositionEnum.BOTTOM) {}
    public GridCellPosition(Vector3 position, GridCellPositionEnum positionE) {
        Position = position;
        PositionE = positionE;
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
}
