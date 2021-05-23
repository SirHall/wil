using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWon : GlobalEvent<GameWon>, System.IDisposable
{
    public int warningAmt;
    public float warningTime;
}