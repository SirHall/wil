using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControlEvent : GlobalEvent<SoundControlEvent>, System.IDisposable 
{
    public HeadInput headInput;
}

/// <summary>
/// Stores a value for the heads input coordinates
/// </summary>
public struct HeadInput 
{
    public Vector3 dir;
}
