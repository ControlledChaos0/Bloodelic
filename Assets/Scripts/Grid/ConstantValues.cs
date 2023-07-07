using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstantValues
{
    /**Array for defining what sides can connect with which where
    Goes ENUM, EDGE, DIRECTION OF CONNECTION

    For example
    ----------------------
    BOTTOM
                    Level       Up          Down
    Top edge        BOTTOM      FRONT       BACK
    Bottom edge     "           BACK        FRONT
    Left edge       "           LEFT        RIGHT
    Right edge      "           RIGHT       LEFT
    **/
    private static GridCellPositionEnum[,,] _positionArray = new GridCellPositionEnum[5, 4, 3]
                                    
                                    {{{GridCellPositionEnum.BOTTOM, GridCellPositionEnum.FRONT, GridCellPositionEnum.BACK},
                                    {GridCellPositionEnum.BOTTOM, GridCellPositionEnum.BACK, GridCellPositionEnum.FRONT},
                                    {GridCellPositionEnum.BOTTOM, GridCellPositionEnum.LEFT, GridCellPositionEnum.RIGHT},
                                    {GridCellPositionEnum.BOTTOM, GridCellPositionEnum.RIGHT, GridCellPositionEnum.LEFT}},
                                    
                                    {{GridCellPositionEnum.FRONT, GridCellPositionEnum.ERROR, GridCellPositionEnum.BOTTOM},
                                    {GridCellPositionEnum.FRONT, GridCellPositionEnum.BOTTOM, GridCellPositionEnum.ERROR},
                                    {GridCellPositionEnum.FRONT, GridCellPositionEnum.LEFT, GridCellPositionEnum.RIGHT},
                                    {GridCellPositionEnum.FRONT, GridCellPositionEnum.RIGHT, GridCellPositionEnum.LEFT}},
                                    
                                    {{GridCellPositionEnum.BACK, GridCellPositionEnum.ERROR, GridCellPositionEnum.BOTTOM},
                                    {GridCellPositionEnum.BACK, GridCellPositionEnum.BOTTOM, GridCellPositionEnum.ERROR},
                                    {GridCellPositionEnum.BACK, GridCellPositionEnum.RIGHT, GridCellPositionEnum.LEFT},
                                    {GridCellPositionEnum.BACK, GridCellPositionEnum.LEFT, GridCellPositionEnum.RIGHT}},
                                    
                                    {{GridCellPositionEnum.RIGHT, GridCellPositionEnum.ERROR, GridCellPositionEnum.BOTTOM},
                                    {GridCellPositionEnum.RIGHT, GridCellPositionEnum.BOTTOM, GridCellPositionEnum.ERROR},
                                    {GridCellPositionEnum.RIGHT, GridCellPositionEnum.FRONT, GridCellPositionEnum.BACK},
                                    {GridCellPositionEnum.RIGHT, GridCellPositionEnum.BACK, GridCellPositionEnum.FRONT}},
                                    
                                    {{GridCellPositionEnum.LEFT, GridCellPositionEnum.ERROR, GridCellPositionEnum.BOTTOM},
                                    {GridCellPositionEnum.LEFT, GridCellPositionEnum.BOTTOM, GridCellPositionEnum.ERROR},
                                    {GridCellPositionEnum.LEFT, GridCellPositionEnum.BACK, GridCellPositionEnum.FRONT},
                                    {GridCellPositionEnum.LEFT, GridCellPositionEnum.FRONT, GridCellPositionEnum.BACK}},
                                    };

    public static GridCellPositionEnum GetPositionalArray(int enumValue, int edge, int dir) {
        return _positionArray[enumValue, edge, dir];
    }
}
