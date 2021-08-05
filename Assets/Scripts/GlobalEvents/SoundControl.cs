using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControlEvent : GlobalEvent<SoundControlEvent>, System.IDisposable 
{
    public HeadInput headInput;
}

public struct HeadInput {
    public Vector2 dir;
}