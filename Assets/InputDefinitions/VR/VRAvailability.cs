using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRAvailability : InputAvailability
{
    private List<InputDevice> headsetDevices = new List<InputDevice>();

    protected override void Start() => base.Start();

    protected override bool CheckAvailability()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, headsetDevices);
        // There are VR headsets if we have atleast one connected
        return headsetDevices.Count > 0;
    }
}
