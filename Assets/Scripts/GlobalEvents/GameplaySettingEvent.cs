using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySettingEvent : GlobalEvent<GameplaySettingEvent>, System.IDisposable
{
    public GameSettings settings;
}

public struct GameSettings
{
    public bool bobbing;
    public float warmupTime;
    public bool introStart;
}