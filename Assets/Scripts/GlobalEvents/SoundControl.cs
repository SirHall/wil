using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControlEvent : GlobalEvent<SoundControlEvent>, System.IDisposable 
{
    public HeadInput headInput;
    public GameInput gameInput;
}

public struct HeadInput {
    public Vector3 dir;
}

public struct GameInput {
    public GameState state;
}