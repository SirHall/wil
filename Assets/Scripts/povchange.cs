using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class povchange : MonoBehaviour
{
    [FormerlySerializedAs("gameObj1")]
    public GameObject keyboardRig;
    [FormerlySerializedAs("gameObj2")]
    public GameObject vrRig;

    private List<InputDevice> headsetDevices = new List<InputDevice>();
    private InputDevice activeHeadset;

    float timePassed = 0;
    // Start is called before the first frame update
    void Awake()
    {
        InitialiseHeadset();
    }

    private void InitialiseHeadset()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, headsetDevices);

        if (headsetDevices.Count > 0) activeHeadset = headsetDevices[0];

        if (activeHeadset.isValid)
        {
            // Debug.Log("Detected VR");
            keyboardRig.SetActive(false);
            vrRig.SetActive(true);

        }
        else
        {
            // Debug.Log("Not detected VR");
            keyboardRig.SetActive(true);
            vrRig.SetActive(false);
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
