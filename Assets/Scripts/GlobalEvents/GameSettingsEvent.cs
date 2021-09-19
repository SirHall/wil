using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsEvent : GlobalEvent<GameSettingsEvent>, System.IDisposable
{
    public GameplaySettings gameplaySettings;
    public AudioSettings audioSettings;
    public PerformanceSettings performanceSettings;
}

public struct GameplaySettings
{
    public bool bobbing;
    public float warmupTime;
    public bool introStart;
    public WaterVisibility waterVisibility;
}

public struct AudioSettings
{
    public float audioLevel;
}

public struct PerformanceSettings
{
    public bool isTerrain;
    public bool isCoral;
}