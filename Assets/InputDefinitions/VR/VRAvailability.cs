using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRAvailability : MonoBehaviour
{
    private List<InputDevice> headsetDevices = new List<InputDevice>();

    void Start()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, headsetDevices);
        // There are no VR headsets
        if (headsetDevices.Count == 0)
            Destroy(gameObject);
    }
}
