using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEndTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        // Tell the world that the player has reached the end
        using (var e = WaveEndEvent.Get()) { }
    }
}
