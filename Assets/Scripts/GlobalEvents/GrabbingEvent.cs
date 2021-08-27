using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftGrabbingEvent : GlobalEvent<LeftGrabbingEvent>, System.IDisposable
{
    public GrabbingLeft grabLeftInput;
}
public struct GrabbingLeft
{
    public bool isLeftGrabbing;
}

public class RightGrabbingEvent : GlobalEvent<RightGrabbingEvent>, System.IDisposable
{
    public GrabbingRight grabRightInput;
}
public struct GrabbingRight
{
    public bool isRightGrabbing;
}