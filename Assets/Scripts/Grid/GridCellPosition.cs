using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellPosition
{
    public GridCellPositionEnum PositionE {
        get;
        set;
    }
    public Vector3 Position{
        get;
        set;
    }

    public GridCellPosition() : this(Vector3.up, GridCellPositionEnum.BOTTOM) {
    }
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
        hash += 89 * hash + (this.Position != null ? this.Position.GetHashCode() : 0);
        hash += 89 * hash + (PositionE.GetHashCode());
        return hash;
    }
}
