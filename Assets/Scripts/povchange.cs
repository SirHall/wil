using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class povchange : MonoBehaviour
{
    public GameObject gameObj1;
    public GameObject gameObj2;

    bool detected = false;

    // Start is called before the first frame update
    void Start()
    {
       
       
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running){
                detected = true;
            }
        }

        if (detected)
        {
            Debug.Log("Detected VR");
            gameObj1.SetActive(false);
            gameObj2.SetActive(true);
            
        }
        else {
            Debug.Log("Not detected VR");
            gameObj1.SetActive(true);
            gameObj2.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
