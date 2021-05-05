using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class HandManager : MonoBehaviour
{
    
    [Tooltip("The hand prefab model")]
    public GameObject handPrefab;

    [SerializeField]
    [Tooltip("Contains the visible hand model that the user can see")]
    private GameObject visibleHandModel;

    [Tooltip("Characteristics of current gameobject device you want to get")]
    public InputDeviceCharacteristics deviceCharacteristics;

    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice currentDevice;

    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseHands();
    }

    /// <summary>
    /// Set up the hands to contain a model an get animation if device exists
    /// </summary>
    private void InitialiseHands() 
    {
        InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);

        if (devices.Count > 0) 
        {
            currentDevice = devices[0];
            visibleHandModel = Instantiate(handPrefab, transform);
            handAnimator = visibleHandModel.GetComponent<Animator>();
        }
        
    }

    /// <summary>
    /// Set animation based on controller button value. 
    /// Example: If grip is completely pressed in, animate the hand to be full grip based on trigger value.
    /// </summary>
    private void HandAnimation() 
    {
        // Trigger Animation
        // Animate hand based on trigger value
        if (devices[0].TryGetFeatureValue(CommonUsages.trigger, out float triggervalue))
            handAnimator.SetFloat("Trigger", triggervalue);
        else
            handAnimator.SetFloat("Trigger", 0);

        // Grip Animation
        // Animate hand based on grip value
        if (devices[0].TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
            handAnimator.SetFloat("Grip", gripvalue);
        else
            handAnimator.SetFloat("Grip", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentDevice.isValid) 
        {
            // Continue to initialise hand if not yet found
            InitialiseHands();
            visibleHandModel.SetActive(false);
            return;
        }

        // Controller hand has been found
        visibleHandModel.SetActive(true);
        HandAnimation();
    }
}
