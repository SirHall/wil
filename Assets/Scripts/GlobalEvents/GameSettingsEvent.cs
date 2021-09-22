using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsEvent : GlobalEvent<GameSettingsEvent>, System.IDisposable
{
    public GameplaySettings gameplaySettings;
    public AudioSettings audioSettings;
    public PerformanceSettings performanceSettings;
}
/// <summary>
/// Stores a value for each option in the Gameplay Settings.
/// </summary>
public struct GameplaySettings
{
    public bool bobbing;
    public float warmupTime;
    public bool introStart;
    public WaterVisibility waterVisibility;
}

/// <summary>
/// Stores a value for each option in the Audio Settings.
/// </summary>
public struct AudioSettings
{
    public float audioLevel;
}

/// <summary>
/// Stores a value for each option in the Performance Settings.
/// </summary>
public struct PerformanceSettings
{
    public bool isTerrain;
    public bool isCoral;
}