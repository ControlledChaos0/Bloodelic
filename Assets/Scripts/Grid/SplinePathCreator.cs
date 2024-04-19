using System;
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
        Spline prevSpline = splineContainer.Spline;
        Spline currSpline = splineContainer.AddSpline();
        GridCell currCell = path.PopFront();
        GridCell prevCell = currCell;
        Vector3 worldPos = currCell.transform.position;

        BezierKnot prevKnot = new BezierKnot(Vector3.zero, Vector3.zero, Vector3.zero, prevCell.transform.rotation);
        prevSpline.Add(prevKnot);
        BezierKnot currKnot;

        while (path.Count != 0) {
            currCell = path.PopFront();
            currKnot = new(currCell.transform.position - worldPos, Vector3.zero, Vector3.zero, currCell.transform.rotation);

            BezierKnot[] knots = CurrKnotCheck(currKnot, prevKnot, currCell, prevCell);
            currKnot = knots[0];
            prevKnot = knots[1];

            prevSpline.SetKnot(0, prevKnot);
            prevSpline.Add(currKnot);
            if (path.Count != 0) {
                currSpline.Add(currKnot);
            }

            prevKnot = currKnot;
            prevCell = currCell;

            prevSpline = currSpline;
            if (path.Count > 1) {
                currSpline = splineContainer.AddSpline();
            }
        }
    }

    private static BezierKnot[] CurrKnotCheck(BezierKnot currKnot, BezierKnot prevKnot, GridCell currCell, GridCell prevCell) {
        BezierKnot editedCurrKnot = currKnot;
        BezierKnot editedPrevKnot = prevKnot;

        int edge = -1;
        for (int i = 0; i < currCell.Neighbors.Length; i++) {
            if (currCell.Neighbors[i].Equals(prevCell)) {
                edge = i;
                break;
            }
        }
        Quaternion currAdditionalRot;
        switch (edge) {
            case 0:
                currAdditionalRot = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case 1:
                currAdditionalRot = Quaternion.identity;
                break;
            case 2:
                currAdditionalRot = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case 3:
                currAdditionalRot = Quaternion.Euler(new Vector3(0, 270, 0));
                break;
            default:
                throw new ArgumentOutOfRangeException("Edge number out of range");
        }
        editedCurrKnot.Rotation *= currAdditionalRot;

        GridCellPositionEnum prevEnum = prevCell.Position.PositionE;
        GridCellPositionEnum currEnum = currCell.Position.PositionE;
        int dir = -1;
        for (int i = 0; i < 3; i++) {
            if (prevEnum == ConstantValues.GetPositionalArray((int)currEnum, edge, i)) {
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
                editedPrevKnot.TangentOut = new(upTangent.x, upTangent.y, upTangent.z);
                editedCurrKnot.TangentIn = new(-upTangent.x, -upTangent.y, -upTangent.z);

                //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
                break;
            //DOWN
            case 2:
                editedPrevKnot.TangentOut = new(downTangent.x, downTangent.y, downTangent.z);
                editedCurrKnot.TangentIn = new(-downTangent.x, -downTangent.y, -downTangent.z);

                //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
                break;
            default:
                throw new ArgumentOutOfRangeException("Direction number out of range");
        }
        BezierKnot[] knots = new BezierKnot[2] {editedCurrKnot, EditPrevKnot(editedCurrKnot, editedPrevKnot, currCell, prevCell)};
        return knots;
    }
    
    private static BezierKnot EditPrevKnot(BezierKnot currKnot, BezierKnot prevKnot, GridCell currCell, GridCell prevCell) {
        BezierKnot editedCurrKnot = currKnot;
        BezierKnot editedPrevKnot = prevKnot;

        int edge = -1;
        for (int i = 0; i < prevCell.Neighbors.Length; i++) {
            if (prevCell.Neighbors[i].Equals(currCell)) {
                edge = i;
                break;
            }
        }
        Quaternion baseRot = Quaternion.Inverse(prevCell.transform.rotation) * ((Quaternion)prevKnot.Rotation);
        Vector3 fixedEuler = baseRot.eulerAngles;
        Vector3 prevRotEuler;
        switch (edge) {
            case 0:
                prevRotEuler = -fixedEuler;
                break;
            case 1:
                prevRotEuler = (new Vector3(0, 180, 0)) - fixedEuler;
                break;
            case 2:
                prevRotEuler = (new Vector3(0, 270, 0)) - fixedEuler;
                break;
            case 3:
                prevRotEuler = (new Vector3(0, 90, 0)) - fixedEuler;
                break;
            default:
                throw new ArgumentOutOfRangeException("Edge number out of range");
        }
        Quaternion forwardRot = Quaternion.Euler(prevRotEuler);
        Quaternion backwardRot = Quaternion.Euler(-prevRotEuler);
        //prevKnot.Rotation *= prevAdditionalRot;
        Vector3 prevTan = (Vector3)prevKnot.TangentIn;
        editedPrevKnot.Rotation *= forwardRot;
        editedPrevKnot.TangentIn = backwardRot * prevTan;

        GridCellPositionEnum prevEnum = prevCell.Position.PositionE;
        GridCellPositionEnum currEnum = currCell.Position.PositionE;
        // int dir = -1;
        // for (int i = 0; i < 3; i++) {
        //     if (currEnum == ConstantValues.GetPositionalArray((int)prevEnum, edge, i)) {
        //         dir = i;
        //         break;
        //     }
        // }

        // switch (dir) {
        //     //LEVEL
        //     case 0:
        //         //Idk if it's possible to set it to linear, combed the documentation and just confused
        //         //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
        //         break;
        //     //UP
        //     case 1:
        //         prevKnot.TangentOut = new(upTangent.x, upTangent.y, upTangent.z);
        //         currKnot.TangentIn = new(-upTangent.x, -upTangent.y, -upTangent.z);

        //         //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
        //         break;
        //     //DOWN
        //     case 2:
        //         prevKnot.TangentOut = new(downTangent.x, downTangent.y, downTangent.z);
        //         currKnot.TangentIn = new(-downTangent.x, -downTangent.y, -downTangent.z);

        //         //prevKnot.TangentIn = Quaternion.Inverse(prevAdditionalRot) * (Vector3)prevKnot.TangentIn;
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException("Direction number out of range");
        // }
        return editedPrevKnot;
    }
}
