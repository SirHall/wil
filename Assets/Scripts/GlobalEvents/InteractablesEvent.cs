using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesEvent : GlobalEvent<InteractablesEvent>, System.IDisposable
{
    public Interactables interactables;
}

// Stores a value for each possible button in the game.
// A much better alternative to using 'floating strings'.
public enum Interactables
{
    None, // A simple default value, will print a reminder to the log to set the interactables value (TODO)
    Surfboard, // Surfboard interactables to control it using the hands

}
