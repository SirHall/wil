using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualControlEvent : GlobalEvent<VisualControlEvent>, System.IDisposable
{
    public Vector3 dir;
}
public class WaterScreenEvent : GlobalEvent<WaterScreenEvent>, System.IDisposable
{
    public float alphaValue;
}