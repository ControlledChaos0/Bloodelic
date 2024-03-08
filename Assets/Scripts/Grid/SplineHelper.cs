using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SplineHelper
{
    public static Quaternion ConvertSplineQuat(quaternion fakeQuat) {
        float4 values = fakeQuat.value;
        return new Quaternion(values.x, values.y, values.z, values.w);
    }
}
