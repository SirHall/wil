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
/// <summary>
/// Stores a value for each possible interactable in the game.
/// </summary>
public enum Interactables
{
    None, // Default value
    Surfboard, // Surfboard interactable
    Water, // Water interactable
}
