using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class HandManager : MonoBehaviour
{
    public GameObject handPrefab;
    private GameObject visibleHandModel;

    public InputDeviceCharacteristics deviceCharacteristics;
    private List<InputDevice> devices = new List<InputDevice>();
    // Start is called before the first frame update
    void Start()
    {
        InitialiseHands();
    }

    private void InitialiseHands() 
    {
        InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);
        visibleHandModel = Instantiate(handPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!devices[0].isValid)
            InitialiseHands();
        visibleHandModel.SetActive(false);


        visibleHandModel.SetActive(true);
        
    }
}
