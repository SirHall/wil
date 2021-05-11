using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class that acts as the input event
public class BoardControlEvent : GlobalEvent<BoardControlEvent>, System.IDisposable
{
    public BoardInput input;
}

// This is a simple structure that holds the input data itself.
// This is used so that each input receptor or listener can also hold the input
// data locally without needing more than one assignment.
public struct BoardInput
{
    public Vector2 dir;
}
