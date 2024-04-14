using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHelper
{
    public static bool CheckGrid(GameObject gO, Vector3 cameraForward) {
        //If not a gridCell, just quit out
        //Grid is layer 3
        if (gO.layer != 3) {
            //Debug.Log($"GO: {gO.name}; Layer: {gO.layer}; GridMask: {ConstantValues.GridMask.value}");
            return false;
        }
        //If the gridcell is facing towards the camera, keep going to distance checking
        if (Vector3.Dot(cameraForward, gO.transform.up) < 0) {
            return false;
        }
        //If gridcell is facing away from the camera, return true and skip distance checking
        return true;
    }
}
