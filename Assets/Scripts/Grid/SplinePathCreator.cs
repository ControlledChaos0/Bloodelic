using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePathCreator : MonoBehaviour
{
    private static GameObject sSplinePrefab;

    [SerializeField]
    private GameObject splinePrefab;
    [SerializeField]
    private static Vector3 upTangent = new(0, 0, .3f);
    [SerializeField]
    private static Vector3 downTangent = new(0, 0, .4f);

    private void Awake() {
        sSplinePrefab = splinePrefab;
    }
    
    public static SplineContainer CreateSplinePath(GridPath path) {
        ArgumentNullExceptionUse.ThrowIfNull(path);
        if (path.Count <= 1) {
            return null;
        }

        GridPath copy = new(path.Path);

        GameObject splineObj = Instantiate(sSplinePrefab, copy.GetFront().transform.position, Quaternion.identity);
        SplineContainer splineContainer = splineObj.GetComponent<SplineContainer>();

        EditSplineContainer(splineContainer, copy);

        return splineContainer;
    }

    private static void EditSplineContainer(SplineContainer splineContainer, GridPath path) {
        Spline spline = splineContainer.Spline;
        GridCell currCell = path.PopFront();
        GridCell prevCell = currCell;
        Vector3 worldPos = currCell.transform.position;

        BezierKnot prevKnot = new BezierKnot(Vector3.zero, Vector3.zero, Vector3.zero, prevCell.transform.rotation);
        spline.Add(prevKnot);
        BezierKnot currKnot;
        int prevIndex = 0;

        while (path.Count != 0) {
            currCell = path.PopFront();
            currKnot = new(currCell.transform.position - worldPos, Vector3.zero, Vector3.zero, currCell.transform.rotation);

            int edge = -1;
            for (int i = 0; i < prevCell.Neighbors.Length; i++) {
                if (prevCell.Neighbors[i].Equals(currCell)) {
                    edge = i;
                    break;
                }
            }
            Quaternion prevAdditionalRot;
            Quaternion prevTanRot = Quaternion.identity;
            switch (edge) {
                case 0:
                    prevAdditionalRot = Quaternion.identity;
                    prevTanRot = Quaternion.identity;
                    break;
                case 1:
                    prevAdditionalRot = Quaternion.Euler(new Vector3(0, 180, 0));
                    prevTanRot = Quaternion.identity;
                    break;
                case 2:
                    prevAdditionalRot = Quaternion.Euler(new Vector3(0, 270, 0));
                    //prevTanRot = Quaternion.AngleAxis(-90, Vector3.up);
                    break;
                case 3:
                    prevAdditionalRot = Quaternion.Euler(new Vector3(0, 90, 0));
                    //prevTanRot = Quaternion.AngleAxis(90, Vector3.up);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Edge number out of range");
            }
            //prevKnot.Rotation *= prevAdditionalRot;
            Vector3 prevTan = (Vector3)prevKnot.TangentIn;
            prevKnot.TangentIn = prevTanRot * prevTan;

            GridCellPositionEnum prevEnum = prevCell.Position.PositionE;
            GridCellPositionEnum currEnum = currCell.Position.PositionE;
            int dir = -1;
            for (int i = 0; i < 3; i++) {
                if (currEnum == ConstantValues.GetPositionalArray((int)prevEnum, edge, i)) {
                    dir = i;
                    break;
                }
            }

            switch (dir) {
                //LEVEL
                case 0:
                    //Idk if it's possible to set it to linear, combed the documentation and just confused
                    //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
                    break;
                //UP
                case 1:
                    prevKnot.TangentOut = new(upTangent.x, upTangent.y, upTangent.z);
                    currKnot.TangentIn = new(-upTangent.x, -upTangent.y, -upTangent.z);

                    //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
                    break;
                //DOWN
                case 2:
                    prevKnot.TangentOut = new(downTangent.x, downTangent.y, downTangent.z);
                    currKnot.TangentIn = new(-downTangent.x, -downTangent.y, -downTangent.z);

                    //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Direction number out of range");
            }
            //prevKnot.Rotation = Quaternion.Inverse(prevAdditionalRot) * Quaternion.Normalize((Quaternion)prevKnot.Rotation) * prevAdditionalRot;

            spline.SetKnot(prevIndex, prevKnot);
            spline.Add(currKnot);

            prevKnot = currKnot;
            prevCell = currCell;

            prevIndex++;
        }
    }

    private static BezierKnot EditCurrKnot() {

    }
    
    private static BezierKnot EditPrevKnot() {
        
    }
}
