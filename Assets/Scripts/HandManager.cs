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

    Interactables interaction = new Interactables();

    void OnEnable() {
        InteractablesEvent.RegisterListener(OnGripControlEvent);
    }

    void OnDisable() {
        InteractablesEvent.UnregisterListener(OnGripControlEvent);
    }
    // A controller has announced new data
    void OnGripControlEvent(InteractablesEvent e) {
        interaction = e.interactables;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseHands();
    }

    
    // Update is called once per frame
    void Update()
    {
        if (!currentDevice.isValid) 
        {
            // Continue to initialise hand if not yet found
            InitialiseHands();
            return;
        }

        // Controller hand has been found

        // Toggle Active
        if (!visibleHandModel.activeSelf) 
        {
            visibleHandModel.SetActive(true);
        }

        // Run Animations
        HandAnimation();
        InteractableGripping();
    }

    /// <summary>
    /// Set up the hands to contain a model an get animation if device exists
    /// </summary>
    private void InitialiseHands() {
        InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);
        if (devices.Count > 0) {
            Vector3 pos;
            Quaternion rot;
            visibleHandModel = new GameObject();
            currentDevice = devices[0];

            //if (currentDevice.TryGetFeatureValue(CommonUsages.devicePosition, out pos)) {
            //    this.transform.position = pos;
            //}
            //if (currentDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rot)) {
            //    this.transform.rotation = rot;
            //}

            visibleHandModel = Instantiate(handPrefab, transform);
            
            //print("1: " + pos);
            //print("2: " + visibleHandModel.transform.position);
            handAnimator = visibleHandModel.GetComponent<Animator>();
        }

    }

    /// <summary>
    /// Set animation based on controller button value. 
    /// Example: If grip is completely pressed in, animate the hand to be full grip based on trigger value.
    /// </summary>
    private void HandAnimation() {
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
    /// <summary>
    /// Conditions to check if player is gripping while in contact with an interactable object. 
    /// </summary>
    private void InteractableGripping() 
    {
        if (devices[0].TryGetFeatureValue(CommonUsages.grip, out float gripvalue)) 
        {
            if (gripvalue == 1 && interaction == Interactables.Surfboard) 
            {
                print("Player is gripping side of board");
            }
        }
    }
}
