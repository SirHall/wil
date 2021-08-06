using System;
using System.Collections;
using System.Collections.Generic;
using Excessives.Unity;
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

    // GLSL-style vector component swizzling :)
    #region Vector Swizzling

    //--- V2 -> V2 ---///
    public static Vector2 xx(this Vector2 v) => new Vector2(v.x, v.x);
    public static Vector2 xy(this Vector2 v) => new Vector2(v.x, v.y);
    public static Vector2 yx(this Vector2 v) => new Vector2(v.y, v.x);
    public static Vector2 yy(this Vector2 v) => new Vector2(v.y, v.y);

    //--- V3 -> V2 ---//
    public static Vector2 xx(this Vector3 v) => new Vector2(v.x, v.x);
    public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
    public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);
    public static Vector2 yx(this Vector3 v) => new Vector2(v.y, v.x);
    public static Vector2 yy(this Vector3 v) => new Vector2(v.y, v.y);
    public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);
    public static Vector2 zx(this Vector3 v) => new Vector2(v.z, v.x);
    public static Vector2 zy(this Vector3 v) => new Vector2(v.z, v.y);
    public static Vector2 zz(this Vector3 v) => new Vector2(v.z, v.z);

    //--- V2 -> V3 ---//
    public static Vector3 xxx(this Vector2 v) => new Vector3(v.x, v.x, v.x);
    public static Vector3 xxy(this Vector2 v) => new Vector3(v.x, v.x, v.y);
    public static Vector3 xyx(this Vector2 v) => new Vector3(v.x, v.y, v.x);
    public static Vector3 xyy(this Vector2 v) => new Vector3(v.x, v.y, v.y);
    public static Vector3 yxx(this Vector2 v) => new Vector3(v.y, v.x, v.x);
    public static Vector3 yxy(this Vector2 v) => new Vector3(v.y, v.x, v.y);
    public static Vector3 yyx(this Vector2 v) => new Vector3(v.y, v.y, v.x);
    public static Vector3 yyy(this Vector2 v) => new Vector3(v.y, v.y, v.y);

    //--- V3 -> V3 ---//
    // I couldn't be bothered to write these by hand, or write a generator so I copied and modified the code from here:
    // https://www.reddit.com/r/Unity3D/comments/2l5331/wrote_a_little_thing_that_adds_swizzling_to/
    public static Vector3 xxx(this Vector3 v) => new Vector3(v.x, v.x, v.x);
    public static Vector3 yxx(this Vector3 v) => new Vector3(v.y, v.x, v.x);
    public static Vector3 zxx(this Vector3 v) => new Vector3(v.z, v.x, v.x);
    public static Vector3 xyx(this Vector3 v) => new Vector3(v.x, v.y, v.x);
    public static Vector3 yyx(this Vector3 v) => new Vector3(v.y, v.y, v.x);
    public static Vector3 zyx(this Vector3 v) => new Vector3(v.z, v.y, v.x);
    public static Vector3 xzx(this Vector3 v) => new Vector3(v.x, v.z, v.x);
    public static Vector3 yzx(this Vector3 v) => new Vector3(v.y, v.z, v.x);
    public static Vector3 zzx(this Vector3 v) => new Vector3(v.z, v.z, v.x);
    public static Vector3 xxy(this Vector3 v) => new Vector3(v.x, v.x, v.y);
    public static Vector3 yxy(this Vector3 v) => new Vector3(v.y, v.x, v.y);
    public static Vector3 zxy(this Vector3 v) => new Vector3(v.z, v.x, v.y);
    public static Vector3 xyy(this Vector3 v) => new Vector3(v.x, v.y, v.y);
    public static Vector3 yyy(this Vector3 v) => new Vector3(v.y, v.y, v.y);
    public static Vector3 zyy(this Vector3 v) => new Vector3(v.z, v.y, v.y);
    public static Vector3 xzy(this Vector3 v) => new Vector3(v.x, v.z, v.y);
    public static Vector3 yzy(this Vector3 v) => new Vector3(v.y, v.z, v.y);
    public static Vector3 zzy(this Vector3 v) => new Vector3(v.z, v.z, v.y);
    public static Vector3 xxz(this Vector3 v) => new Vector3(v.x, v.x, v.z);
    public static Vector3 yxz(this Vector3 v) => new Vector3(v.y, v.x, v.z);
    public static Vector3 zxz(this Vector3 v) => new Vector3(v.z, v.x, v.z);
    public static Vector3 xyz(this Vector3 v) => new Vector3(v.x, v.y, v.z);
    public static Vector3 yyz(this Vector3 v) => new Vector3(v.y, v.y, v.z);
    public static Vector3 zyz(this Vector3 v) => new Vector3(v.z, v.y, v.z);
    public static Vector3 xzz(this Vector3 v) => new Vector3(v.x, v.z, v.z);
    public static Vector3 yzz(this Vector3 v) => new Vector3(v.y, v.z, v.z);
    public static Vector3 zzz(this Vector3 v) => new Vector3(v.z, v.z, v.z);

    public static Vector3 WithX(this Vector3 v, Func<Vector3, float> f) => v.WithX(f(v));
    public static Vector3 WithY(this Vector3 v, Func<Vector3, float> f) => v.WithY(f(v));
    public static Vector3 WithZ(this Vector3 v, Func<Vector3, float> f) => v.WithZ(f(v));
    public static Vector3 WithX(this Vector3 v, Func<float, float> f) => v.WithX(f(v.x));
    public static Vector3 WithY(this Vector3 v, Func<float, float> f) => v.WithY(f(v.y));
    public static Vector3 WithZ(this Vector3 v, Func<float, float> f) => v.WithZ(f(v.z));

    public static Vector2 WithX(this Vector2 v, Func<Vector2, float> f) => v.WithX(f(v));
    public static Vector2 WithY(this Vector2 v, Func<Vector2, float> f) => v.WithY(f(v));
    public static Vector2 WithX(this Vector2 v, Func<float, float> f) => v.WithX(f(v.x));
    public static Vector2 WithY(this Vector2 v, Func<float, float> f) => v.WithY(f(v.y));

    #endregion
}
