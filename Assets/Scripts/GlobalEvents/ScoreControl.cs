using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreControlEvent : GlobalEvent<ScoreControlEvent>, System.IDisposable 
{
    public ScoreInput input;
}

// This is a simple structure that holds the input data itself.
// This is used so that each input receptor or listener can also hold the input
// data locally without needing more than one assignment.
public struct ScoreInput {
    public int warningAmt;
    public float warningTime;
    // Can expand upon warnings
}

