using UnityEngine;

/// <summary>
/// This object carries the wave setting data from this scene to the next one
/// </summary>
public class BarrelSettings : MonoBehaviour
{
    public static BarrelSettings Instance { get; private set; }

    public float Radius { get; set; }
    public float Length { get; set; }
    public float Arc { get; set; }

    void Awake() => DontDestroyOnLoad(gameObject);

    void OnEnable() { Instance = this; }
    void OnDisable() { Instance = null; }

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
                squash = 1.0f
            };
        }
    }
}
