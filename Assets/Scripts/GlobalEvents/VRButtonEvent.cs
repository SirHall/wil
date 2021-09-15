using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButtonEvent : GlobalEvent<VRButtonEvent>, System.IDisposable
{
    public VRButtons button;
}

// Stores a value for each possible button in the game.
// A much better alternative to using 'floating strings'.
public enum VRButtons
{
    None, // A simple default value, will print a reminder to the log to set the button's value (TODO)
    Test, // A button that simply makes a noise, for testing only
    MainMenu_Start, // Start button in the main menu
    Exit, // Any button which attemptes to close the game
    Designer, // Go to the barrel designer UI
    Surf, // Start surf scene

    Designer_IncreaseRadius, // Increase the radius of the barrel
    Designer_DecreaseRadius, // Decrease the radius of the barrel
    Designer_Longer, // Increase the length of the barrel
    Designer_Shorter, // Decrease the length of the barrel

    MainMenu, // Go back to the main menu
    Retry, // Re-attempt the current barrel

    SurfDir, // Toggle the surfing direction

    Replay, // Replay our last surfing run on the corresponding screen

    Options_Gameplay_Bobbing, // Toggle if the player and waves are bobbing
    Options_Gameplay_IncreaseWarmup, // Increase warmup time
    Options_Gameplay_DecreaseWarmup, // Decrease warmup time
    Options_Gameplay_IntroStart, // Toggle if the player will experience the intro

    Options_Audio_Min,
    Options_Audio_Max,
    Options_Audio_DecreaseVolume,
    Options_Audio_IncreaseVolume,

    Options_GameplaySwitch,
    Options_AudioSwitch,
    Options_PerformanceSwitch,

    Options_Gameplay_IncreaseCoral,
    Options_Gameplay_DecreaseCoral,

    Options_Performance_Terrain,
    Options_Performance_Coral
}