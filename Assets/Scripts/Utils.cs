using System.Collections;
using System.Collections.Generic;
using Excessives;
using UnityEngine;

public static class Utils
{
    // Rotate a Vector3 point around some pivot on a given axis
    public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rot) =>
        (rot * (point - pivot)) + pivot;

    // A sine wave (cosine) that is normalized to map x -> y between the
    // range (0, 1) such that x = 0 : y = 1 and x = 1 : y = 1
    public static float nSin(float x) => Mathf.Cos(x * Mathf.PI + Mathf.PI) * 0.5f + 0.5f;

    public static float Tau { get => (float)MathE.TAU; }
}
