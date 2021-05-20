using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HeadSet : MonoBehaviour
{
    [Tooltip("This is the transform to which the VR system writes the head position in the real-world")]
    [SerializeField] Transform headPosInput;

    [Tooltip("The head's initial position will be calibrated after this many frames. Doesn't really need to be anything above 1.")]
    [SerializeField] int ticksDelay = 5;

    [Tooltip("The current head position relative to the initial position will be set to this transform")]
    [SerializeField] Transform headPosOutput;

    // [SerializeField] Transform headPositionTransform;

    Vector3 offset;

    void Update()
    {
        if(ticksDelay >= 0)
            ticksDelay--;

        if(ticksDelay == 0)
        {
            offset = headPosOutput.localPosition - headPosInput.localPosition;
            offset.y = 0.0f;
        }

        if(ticksDelay < 0)
        {
            headPosOutput.localPosition = headPosInput.localPosition + offset;
            headPosOutput.localRotation = headPosInput.localRotation;
        }
    }
}
