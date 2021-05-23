using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreControlEvent : GlobalEvent<ScoreControlEvent>, System.IDisposable
{
    public int warningAmt;
    public float warningTime;
    // Can expand upon warnings
}