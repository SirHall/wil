using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This event only signals that the player has reached the end, as such it need not store any data
/// </summary>
public class WaveEndEvent : GlobalEvent<WaveEndEvent>, System.IDisposable { }