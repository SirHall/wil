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

    public static Vector3 ProjectPoint(Vector3 point, Vector3 linePoint, Vector3 lineDir) => linePoint + Vector3.Project(point - linePoint, lineDir);

    public static void Quit() // Yes, this is how it must be done
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static float Clamp(this float v, Vector2 r) => Mathf.Clamp(v, r.x, r.y);

}
