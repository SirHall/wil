using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    // Gameplay Settings
    [ShowInInspector] public bool Bobbing { get; set; } = true; //Enables the board bobbing
    [ShowInInspector] public float WarmupTime { get; set; } = 5f; //Sets the length of time the warmup will last for
    [ShowInInspector] public bool IntroStart { get; set; } = true; //Enables or disables the starting intro
    [ShowInInspector] public WaterVisibility WaterMode { get; set; } = WaterVisibility.Medium; // Sets the coral visibility level (1 - Low, 2 - Medium, 3 - High)

    // Audio Settings
    [ShowInInspector] public float AudioLevel { get; set; } = 100f; // Sets the volume percentage for the overall game audio level

    // Performance Settings
    [ShowInInspector] public bool Terrain { get; set; } = true; // Enables or disables terrain
    [ShowInInspector] public bool Coral { get; set; } = true; // Enables or disables coral


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

    public static void EmitGameSettings()
    {
        if (Instance == null)
            return;

        using (var e = GameSettingsEvent.Get())
        {
            e.gameplaySettings = new GameplaySettings
            {
                bobbing = Instance.Bobbing,
                warmupTime = Instance.WarmupTime,
                introStart = Instance.IntroStart,
                waterVisibility = Instance.WaterMode
            };
            e.audioSettings = new AudioSettings
            {
                audioLevel = Instance.AudioLevel
            };
            e.performanceSettings = new PerformanceSettings
            {
                isTerrain = Instance.Terrain,
                isCoral = Instance.Coral
            };
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
public enum WaterVisibility
{
    Low,
    Medium,
    High
}