using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// This object carries the wave setting data from this scene to the next one
/// </summary>
public class BarrelSettings : MonoBehaviour
{
    public static BarrelSettings Instance { get; private set; }

    [ShowInInspector] public float Radius { get; set; } = 4.0f;
    [ShowInInspector] public float Length { get; set; } = 50.0f;
    [ShowInInspector] public float Arc { get; set; } = 0.8f;
    [ShowInInspector] public RightLeft SurfDir { get; set; } = RightLeft.Right;

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void EmitBarrelSettings()
    {
        if (Instance == null)
            return;

        using (var e = WaveSettingEvent.Get())
        {
            e.settings = new WaveSettings
            {
                radius = Instance.Radius,
                length = Instance.Length,
                arc = Instance.Arc,
                squash = 1.0f,
                surfDir = Instance.SurfDir,
            };
        }
    }
}

public enum RightLeft
{
    Right,
    Left
}