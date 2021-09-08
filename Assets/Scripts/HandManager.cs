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

    [SerializeField]
    [Tooltip("Determines if the hand is the left or right controller")]
    public HandType handType;

    [Tooltip("Characteristics of current gameobject device you want to get")]
    public InputDeviceCharacteristics deviceCharacteristics;

    [Tooltip("Devices that are found with set characteristics")]
    private List<InputDevice> devices = new List<InputDevice>();

    [Tooltip("Primary device found within list of devices")]
    private InputDevice currentDevice;

    [Tooltip("Animator to play hand movements such as gripping")]
    private Animator handAnimator;

    [Tooltip("Is current hand interacting with any object while gripping")]
    private bool isGripInteracting = false;

    [Tooltip("Get which interactable either the left or right hand is interacting with")]
    Interactables leftInteraction, rightInteraction = new Interactables();

    [Tooltip("")]
    Interactables handInteraction = new Interactables();

    [Tooltip("The max unit values the player can move in specified direction")]
    public float maxUp, maxDown;

    [SerializeField]
    [Tooltip("Starting local Cooridnate of the player")]
    private Vector3 startCoordinate;

    [Tooltip("The player's hand position relative to the starting hand position")]
    private Vector3 handPosRel = Vector3.zero;

    public enum HandType
    {
        left,
        right
    }

    void OnEnable() {
        LeftInteractablesEvent.RegisterListener(OnLeftGripControlEvent);
        RightInteractablesEvent.RegisterListener(OnRightGripControlEvent);
    }

    void OnDisable() {
        LeftInteractablesEvent.UnregisterListener(OnLeftGripControlEvent);
        RightInteractablesEvent.RegisterListener(OnRightGripControlEvent);
    }

    // A controller has announced new data
    void OnLeftGripControlEvent(LeftInteractablesEvent e) {
        leftInteraction = e.leftInteractable;
    }

    // A controller has announced new data
    void OnRightGripControlEvent(RightInteractablesEvent e)
    {
        rightInteraction = e.rightInteractable;
    }

    private void CallGlobalEvents()
    {
        
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

        // Set current Hand
        handInteraction = handType == HandType.left ? leftInteraction : rightInteraction;

        // Toggle Active
        if (!visibleHandModel.activeSelf) 
            visibleHandModel.SetActive(true);
        
        HandAnimation();
        SurfboardGripping();
        CallGlobalEvents();
    }

    /// <summary>
    /// Converts hand positional movements into board input with either left or right input. 
    /// </summary>
    /// <param name="handPos">Vector3 of clamped hand position value between 0 and 1</param>
    /// <returns>Vector3 direction value to be used rotate the board based on hand movements</returns>
    private Vector3 HandPosToBoardInput(Vector3 handPos)
    {
        Vector3 dir = Vector3.zero;

        // Cutoff values determin when the hand position values should be ignored and returned as a 0 value 
        float heightCutoff = 0.05f;

        float leftMovementValue = -handPos.y + heightCutoff;
        float rightMovementValue = Mathf.Abs(handPos.y + heightCutoff);

        // Moving
        if (handPos.y >= heightCutoff) { dir.x = leftMovementValue; } //Left
        if (handPos.y <= -heightCutoff) { dir.x = rightMovementValue; } //Right

        // Stationary
        if (handPos.y < heightCutoff && handPos.y > -heightCutoff) { dir.x = 0; } // Side

        return dir;
    }

    /// <summary>
    /// Set up the hands to contain a model an get animation if device exists
    /// </summary>
    private void InitialiseHands() {
        InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);
        if (devices.Count > 0) {

            visibleHandModel = new GameObject();
            currentDevice = devices[0];

            visibleHandModel = Instantiate(handPrefab, transform);

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
    /// Returns a bool value depending of if the player is gripping which interacting with a given type. 
    /// </summary>
    /// <param name="device"></param>
    /// <param name="leftHand"></param>
    /// <param name="rightHand"></param>
    /// <param name="type"></param>
    /// <returns>Bool value depending of if player is gripping while either hand is interacting with type</returns>
    private bool IsGripping(InputDevice device, Interactables currentHand, Interactables type)
    {
        if (device.TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
            if (gripvalue == 1) 
                if(currentHand == type)
                    return true;
        
        return false;
    }

    /// <summary>
    /// Conditions to check if player is gripping while in contact with the surfboard gripping areas. 
    /// </summary>
    private void SurfboardGripping() 
    {
        bool previousInteractionState = isGripInteracting;
        isGripInteracting = IsGripping(devices[0], handInteraction, Interactables.Surfboard);

        if (isGripInteracting)
        {
            // Run once when player starts interacting
            if (previousInteractionState != isGripInteracting)
            {
                startCoordinate = transform.localPosition;

                // Trigger GripInteraction event
                using (var e = GripInteraction.Get()); 
            }
            
            CheckHandPosition();
            using (var e = BoardControlEvent.Get())
                e.input.dir = HandPosToBoardInput(handPosRel);
        }
    }

    /// <summary>
    /// Return positive value that is scaled between 0 and a given max value
    /// </summary>
    /// <param name="value">Difference value based off current position - Starting position</param>
    /// <param name="maxValue">Max difference value for given direction</param>
    /// <returns>Positive float scaled between 0 and given max value</returns>
    private float DirectionScale(float value, float maxValue)
    {
        // Clamp and scale positive value based off given maxValue
        return Mathf.Clamp(Mathf.Abs(value), 0, maxValue) / maxValue;
    }

    /// <summary>
    /// Manages head position values based on current coordinates from start coordinates
    /// </summary>
    void CheckHandPosition()
    {
        if (startCoordinate != Vector3.zero)
        {
            Vector3 diff = transform.localPosition - startCoordinate;

            // Assign variable values based on the direction the player is leaning / standing
            if (diff.y > 0)
                handPosRel.y = DirectionScale(diff.y, maxUp); // Positive
            else
                handPosRel.y = -DirectionScale(diff.y, maxDown); // Negative
        }
    }
}
