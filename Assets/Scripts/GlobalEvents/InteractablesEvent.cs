using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftInteractablesEvent : GlobalEvent<LeftInteractablesEvent>, System.IDisposable
{
    public Interactables leftInteractable;
}
public class RightInteractablesEvent : GlobalEvent<RightInteractablesEvent>, System.IDisposable
{
    public Interactables rightInteractable;
}
// Stores a value for each possible button in the game.
// A much better alternative to using 'floating strings'.
public enum Interactables
{
    None, // A simple default value
    Surfboard, // Surfboard interactables to control it using the hands
    Water,
}
