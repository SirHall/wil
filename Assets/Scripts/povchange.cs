using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class povchange : MonoBehaviour
{
    public GameObject gameObj1;
    public GameObject gameObj2;

    private List<InputDevice> headsetDevices = new List<InputDevice>();
    private InputDevice activeHeadset;

    float timePassed = 0;
    // Start is called before the first frame update
    void Start()
    {
        InitialiseHeadset();
    }

    private void InitialiseHeadset()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, headsetDevices);

        if (headsetDevices.Count > 0) activeHeadset = headsetDevices[0];

        if (activeHeadset.isValid)
        {
            Debug.Log("Detected VR");
            gameObj1.SetActive(false);
            gameObj2.SetActive(true);

        }
        else
        {
            Debug.Log("Not detected VR");
            gameObj1.SetActive(true);
            gameObj2.SetActive(false);
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
