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
}