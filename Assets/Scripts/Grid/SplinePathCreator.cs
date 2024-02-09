using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePathCreator : MonoBehaviour
{
    private static GameObject sSplinePrefab;

    [SerializeField]
    private GameObject splinePrefab;

    private void Awake() {
        sSplinePrefab = splinePrefab;
    }
    
    public static SplineContainer CreateSplinePath(GridPath path) {
        ArgumentNullExceptionUse.ThrowIfNull(path);
        if (path.Count <= 1) {
            return null;
        }

        GameObject splineObj = Instantiate(sSplinePrefab, path.GetFront().transform.position, Quaternion.identity);
        SplineContainer splineContainer = splineObj.GetComponent<SplineContainer>();

        EditSplineContainer(splineContainer, path);

        return splineContainer;
    }

    private static void EditSplineContainer(SplineContainer splineContainer, GridPath path) {
        Spline spline = splineContainer.AddSpline();
        GridCell currCell = path.PopFront();
        GridCell prevCell = currCell;
        Vector3 worldPos = currCell.transform.position;

        BezierKnot prevKnot = new BezierKnot(Vector3.zero, Vector3.zero, Vector3.zero);
        BezierKnot currKnot = prevKnot;
        spline.Add(prevKnot);

        while (path.Count != 0) {
            currCell = path.PopFront();
            currKnot = new BezierKnot(currCell.transform.position - worldPos, Vector3.zero, Vector3.zero);

            int edge = -1;
            for (int i = 0; i < prevCell.Neighbors.Length; i++) {
                if (prevCell.Neighbors[i].Equals(currCell)) {
                    edge = i;
                    break;
                }
            }

            GridCellPositionEnum prevEnum = prevCell.Position.PositionE;
            int dir = -1;
            for (int i = 0; i < 3; i++) {
                if (currCell.Position.PositionE == ConstantValues.GetPositionalArray((int)prevCell.Position.PositionE, edge, i)) {

                }
            }

            switch (dir) {
                default:
                    throw new ArgumentOutOfRangeException("Can't do that");
                    break;
            }
        }
    }
}
