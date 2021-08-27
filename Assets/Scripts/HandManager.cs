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

    public enum HandType
    {
        left,
        right
    }

    [SerializeField]
    [Tooltip("Determines if the hand is the left or right controller")]
    public HandType handType;

    [Tooltip("Characteristics of current gameobject device you want to get")]
    public InputDeviceCharacteristics deviceCharacteristics;

    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice currentDevice;

    private Animator handAnimator;

    private bool isInteracting = false;

    private bool isLeftInteracting = false;
    private bool isRightInteracting = false;

    Interactables leftInteraction = new Interactables();
    Interactables rightInteraction = new Interactables();

    [Tooltip("The max unit values the player can move in specified direction")]
    public float maxUp, maxDown;

    [SerializeField]
    [Tooltip("Starting local Cooridnate of the player")]
    private Vector3 startCoordinate;

    /// <summary>
    /// The player's hand position relative to the starting hand position
    /// </summary>
    private Vector3 handPosRel = Vector3.zero;

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
        if (handType == HandType.left)
        {
            using (var e = LeftGrabbingEvent.Get()) 
                e.grabLeftInput.isLeftGrabbing = isLeftInteracting;
        }
        if (handType == HandType.right)
        {
            using (var e = RightGrabbingEvent.Get())
                e.grabRightInput.isRightGrabbing = isRightInteracting;
        }
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

        CallGlobalEvents();
    }

    private Vector3 HandPosToBoardInput(Vector3 handPos)
    {
        Vector3 dir = Vector3.zero;

        // Apply forward movement if the player leans 40% or greater based on the max forward value.
        // and use 30% on the sides

        // Cutoff values determin when the head position values should be ignored and returned as a 0 value (Essentially making the surfboard stationary)
        // Or when they should return their true values which will be used by the surfboard to move it. 
        float heightCutoff = 0.05f;

        // Moving
        if (handPos.y >= heightCutoff) { dir.x = -handPos.y + heightCutoff; } //Left
        if (handPos.y <= -heightCutoff) { dir.x = Mathf.Abs(handPos.y + heightCutoff); } //Right

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
    /// Conditions to check if player is gripping while in contact with an interactable object. 
    /// </summary>
    private void InteractableGripping() 
    {
        bool previousInteractionState = isInteracting;
        if (devices[0].TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
        {
            if (gripvalue == 1)
            {
                if (leftInteraction == Interactables.Surfboard || rightInteraction == Interactables.Surfboard)
                {
                    if (handType == HandType.left) isLeftInteracting = true;
                    if (handType == HandType.right) isRightInteracting = true;
                    isInteracting = true;
                }
            }
            else
            {
                if (handType == HandType.left) isLeftInteracting = false;
                if (handType == HandType.right) isRightInteracting = false;
                isInteracting = false;
            }
            
        }
        //print("Interactable: " + interaction);
        if (isInteracting)
        {
            if (previousInteractionState != isInteracting)
                startCoordinate = transform.localPosition;
            
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
