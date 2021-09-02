using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLost : GlobalEvent<GameLost>, System.IDisposable
{
    public string cause = "";
}