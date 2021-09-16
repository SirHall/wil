using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    // Gameplay
    [SerializeField] GameObject gameplayHolder;
    [SerializeField] TextMeshPro button_bobbingText;
    [SerializeField] TextMeshPro button_introText;
    [SerializeField] TextMeshPro panel_warmupText;
    [SerializeField] TextMeshPro panel_coralText;

    // Audio
    [SerializeField] GameObject audioHolder;
    [SerializeField] TextMeshPro panel_AudioText;

    // Performance 
    [SerializeField] GameObject performanceHolder;
    [SerializeField] TextMeshPro button_TerrainText;
    [SerializeField] TextMeshPro button_CoralText;

    [SerializeField]
    [Tooltip("Player and wave bobbing")]
    private bool isBobbing = true;

    [SerializeField]
    [Tooltip("Warmup time the player will have before being moved and measured")]
    private float warmupTime = 5;

    [Tooltip("Warmup time incremental value")]
    private float warmupTimeDelta = 5;
    [Tooltip("Warmup time max value")]
    private float warmupTimeMax = 60;
    [Tooltip("Warmup time min value")]
    private float warmupTimeMin = 0;

    [SerializeField]
    [Tooltip("Intro which moves the player towards the barrel")]
    private bool isIntro = true;

    [SerializeField]
    [Tooltip("Global game volume which changes all sounds")]
    private float gameVolume = 100;

    [SerializeField]
    [Tooltip("Toggles the terrain being visible")]
    private bool isTerrain = true;

    [SerializeField]
    [Tooltip("Toggle the LOD coral being visible")]
    private bool isCoral = true;

    private CoralVisibility coralVisibility;

    void OnEnable() => VRButtonEvent.RegisterListener(OnVRButtonEvent);
    void OnDisable() => VRButtonEvent.UnregisterListener(OnVRButtonEvent);

    void Start()
    {
        UpdateStats();
    }

    /// <summary>
    /// Sets local variables and text based on global settings
    /// </summary>
    void UpdateStats()
    {
        // Gameplay
        isBobbing = GameSettings.Instance.Bobbing;
        warmupTime = GameSettings.Instance.WarmupTime;
        isIntro = GameSettings.Instance.IntroStart;
        coralVisibility = GameSettings.Instance.CoralMode;

        // Audio
        gameVolume = GameSettings.Instance.AudioLevel;

        // Performance 
        isTerrain = GameSettings.Instance.Terrain;
        isCoral = GameSettings.Instance.Coral;

        button_TerrainText.text = isTerrain ? "On" : "Off";

        button_CoralText.text = isCoral ? "On" : "Off";

        button_bobbingText.text = isBobbing ? "On" : "Off";
        panel_warmupText.text = warmupTime.ToString() + " Seconds";
        button_introText.text = isIntro ? "On" : "Off";
        panel_AudioText.text = gameVolume.ToString() + " %";
        panel_coralText.text = coralVisibility.ToString();
    }

    /// <summary>
    /// Updates the player bobbing settings
    /// </summary>
    void UpdateBobbing()
    {
        isBobbing = GameSettings.Instance.Bobbing ? false : true;
        GameSettings.Instance.Bobbing = isBobbing;
        button_bobbingText.text = isBobbing ? "On" : "Off";
    }
    /// <summary>
    /// Updates the warmup time settings
    /// </summary>
    /// <param name="time">Float variable for the warmup time</param>
    void UpdateWarmup(float time)
    {
        warmupTime = Mathf.Clamp(time, warmupTimeMin, warmupTimeMax);
        GameSettings.Instance.WarmupTime = warmupTime;
        panel_warmupText.text = warmupTime.ToString() + " Seconds";
    }
    /// <summary>
    /// Updates the player surf intro settings
    /// </summary>
    void UpdateIntro()
    {
        isIntro = GameSettings.Instance.IntroStart ? false : true;
        GameSettings.Instance.IntroStart = isIntro;
        button_introText.text = isIntro ? "On" : "Off";
    }
    /// <summary>
    /// Updates the global audio volume settings
    /// </summary>
    /// <param name="volume">Float value used as the volume</param>
    void UpdateAudio(float volume)
    {
        gameVolume = Mathf.Clamp(volume, 0, 100);
        GameSettings.Instance.AudioLevel = gameVolume;
        AudioListener.volume = Mathf.Clamp(Mathf.InverseLerp(0, 100, gameVolume), 0, 1);
        panel_AudioText.text = gameVolume.ToString() + " %";
    }
    /// <summary>
    /// Toggle between each options menu
    /// </summary>
    /// <param name="active">Gameobject which will be the active option menu</param>
    void ToggleOptions(GameObject active)
    {
        gameplayHolder.SetActive(false);
        audioHolder.SetActive(false);
        performanceHolder.SetActive(false);

        active.SetActive(true);
    }
    /// <summary>
    /// Update the coral visibility through the water
    /// </summary>
    /// <param name="coralValue">Int value which corresponds to the coral visibility enum</param>
    void UpdateCoralLevels(int coralValue)
    {
        int coralLevelLength = System.Enum.GetValues(typeof(CoralVisibility)).Length - 1;

        if ((int)coralVisibility == coralLevelLength && coralValue > (int)coralVisibility)
            return;
        else if ((int)coralVisibility == 0 && coralValue < 0)
            return;

        coralVisibility = (CoralVisibility)coralValue;
        GameSettings.Instance.CoralMode = coralVisibility;
        panel_coralText.text = coralVisibility.ToString();
    }
    /// <summary>
    /// Updates the terrain prefab visibility setting
    /// </summary>
    void UpdateTerrainVisibility()
    {
        isTerrain = GameSettings.Instance.Terrain ? false : true;
        GameSettings.Instance.Terrain = isTerrain;
        button_TerrainText.text = isTerrain ? "On" : "Off";
    }
    /// <summary>
    /// Updates the LOD coral visibility setting
    /// </summary>
    void UpdateCoralVisibility()
    {
        isCoral = GameSettings.Instance.Coral ? false : true;
        GameSettings.Instance.Coral = isCoral;
        button_CoralText.text = isCoral ? "On" : "Off";
    }

    void OnVRButtonEvent(VRButtonEvent e)
    {
        switch (e.button)
        {
            case VRButtons.None:
                Debug.LogError("No event should be fired for a VRButton of type 'None'");
                break;

            case VRButtons.Options_Gameplay_Bobbing: UpdateBobbing(); break;
            case VRButtons.Options_Gameplay_IncreaseWarmup: UpdateWarmup(warmupTime + warmupTimeDelta); break;
            case VRButtons.Options_Gameplay_DecreaseWarmup: UpdateWarmup(warmupTime - warmupTimeDelta); break;
            case VRButtons.Options_Gameplay_IntroStart: UpdateIntro(); break;
            case VRButtons.Options_Gameplay_IncreaseCoral: UpdateCoralLevels((int)coralVisibility + 1); break;
            case VRButtons.Options_Gameplay_DecreaseCoral: UpdateCoralLevels((int)coralVisibility - 1); break;

            case VRButtons.Options_Audio_Min: UpdateAudio(0); break;
            case VRButtons.Options_Audio_Max: UpdateAudio(100); break;
            case VRButtons.Options_Audio_IncreaseVolume: UpdateAudio(gameVolume + 10); break;
            case VRButtons.Options_Audio_DecreaseVolume: UpdateAudio(gameVolume - 10); break;

            case VRButtons.Options_GameplaySwitch: ToggleOptions(gameplayHolder); break;
            case VRButtons.Options_AudioSwitch: ToggleOptions(audioHolder); break;
            case VRButtons.Options_PerformanceSwitch: ToggleOptions(performanceHolder); break;

            case VRButtons.Options_Performance_Terrain: UpdateTerrainVisibility(); break;
            case VRButtons.Options_Performance_Coral: UpdateCoralVisibility(); break;

            default:
                break; // Ignore all other buttons
        }
    }
}
