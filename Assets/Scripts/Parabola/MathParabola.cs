using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Anthony's Parabola class
public static class MathParabola
{
    /// <summary>
    ///  Parabola equation (inversed): y = -(x - h)^2 + k 
    /// </summary>

    public static Vector3 PointOnParabola(Vector3 start, Vector3 end, float height, float interpolation)
    {
        float y = ParabolaY(interpolation * (end.x - start.x), height);

        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        Vector3 mid = Vector3.Lerp(start, end, interpolation);

        return new Vector3(mid.x, mid.y + f(interpolation), mid.z);
    }

    static float ParabolaY(float x, float h)
    {
        return -(x - h) * (x - h);
    }
}
