using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualControlEvent : GlobalEvent<VisualControlEvent>, System.IDisposable {
    public MovementInput input;
}

// This is a simple structure that holds the input data itself.
// This is used so that each input receptor or listener can also hold the input
// data locally without needing more than one assignment.
public struct MovementInput {
    public Vector3 dir;
}