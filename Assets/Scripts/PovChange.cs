using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class PovChange : MonoBehaviour
{
    [FormerlySerializedAs("gameObj1")]
    public GameObject keyboard_Rig;

    [FormerlySerializedAs("gameObj2")]
    public GameObject vr_Rig;

    [Tooltip("Devices found which match a set of characteristics")]
    private List<InputDevice> headsetDevices = new List<InputDevice>();

    [Tooltip("Primary device found within list of devices")]
    private InputDevice activeHeadset;

    [Tooltip("Timer")]
    float timePassed = 0;
    // Start is called before the first frame update
    void Awake()
    {
        InitialiseHeadset();
    }

    private void InitialiseHeadset()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, headsetDevices);

        if (headsetDevices.Count > 0) 
            activeHeadset = headsetDevices[0];

        if (activeHeadset.isValid)
        {
            // Debug.Log("Detected VR");
            keyboard_Rig.SetActive(false);
            vr_Rig.SetActive(true);
        }
        else
        {
            // Debug.Log("Not detected VR");
            keyboard_Rig.SetActive(true);
            vr_Rig.SetActive(false);
        }
    }

    void Update()
    {
        if (!activeHeadset.isValid && timePassed < 3)
        {
            InitialiseHeadset();
            timePassed += Time.deltaTime;
            return;
        }
    }
}
