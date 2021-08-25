using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSettingEvent : GlobalEvent<WaveSettingEvent>, System.IDisposable
{
    public WaveSettings settings;
}

public struct WaveSettings
{
    public float radius;
    public float length;
    public float arc;
    public float squash;
    public RightLeft surfDir;
}