using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAvailability : MonoBehaviour
{
    [Tooltip("How many times per second, should this input system check the availability of its input?")]
    [SerializeField] protected float checkRate = 5.0f;

    public bool Available { get; protected set; } = false;

    protected virtual void Start() => StartCoroutine(CheckLoop());

    IEnumerator CheckLoop()
    {
        // I *hate* doing repeated unecessary divisions
        // It's entirely possible that the Mono JiT compiler may optimize this away if it sees somehow that checkRate
        // doesn't change since the object's in-scene instantiation - but I'm not going to go profile for that >:(
        float waitTime = 1.0f / checkRate;

        while (true)
        {
            Available = CheckAvailability();

            yield return new WaitForSeconds(waitTime);
        }
    }

    // This ought to return the availability of this input system at this point in time
    protected virtual bool CheckAvailability()
    {
        return false;
    }
}
