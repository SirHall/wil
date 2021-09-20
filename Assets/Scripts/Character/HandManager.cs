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

    [Tooltip("The max unit values the player can move in specified direction")]
    public float maxUp, maxDown;

    [SerializeField]
    [Tooltip("Starting local Cooridnate of the player")]
    private Vector3 startCoordinate;

    [SerializeField]
    [Tooltip("The player's hand position relative to the starting hand position")]
    private Vector3 handPosRel = Vector3.zero;

    private bool gripSession;

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
        RightInteractablesEvent.UnregisterListener(OnRightGripControlEvent);
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
            visibleHandModel.SetActive(true);
        
        HandAnimation();
        SurfboardGripping();
        WaterTouching();
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

            if (deviceCharacteristics == InputDeviceCharacteristics.Left) handType = HandType.left;
            else if (deviceCharacteristics == InputDeviceCharacteristics.Right) handType = HandType.right;

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
    private bool IsGrippingType(InputDevice device, Interactables type)
    {
        if (device.TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
        {
            if (gripvalue == 1)
            {
                if (type == Interactables.None) return true;

                if (leftInteraction == type || rightInteraction == type)
                    return true;
            }
        }
        return false;
    }
    private bool IsGripping(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
        {
            if (gripvalue == 1)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Conditions to check if player is gripping while in contact with the surfboard gripping areas. 
    /// </summary>
    private void SurfboardGripping() 
    {
        bool previousInteractionState = isGripInteracting;
        isGripInteracting = IsGrippingType(devices[0], Interactables.Surfboard);
        Vector3 boardInput;

        if (isGripInteracting)
        {
            // Run once when player starts interacting
            if (previousInteractionState != isGripInteracting)
            {
                startCoordinate = transform.localPosition;
                gripSession = true;
                // Trigger GripInteraction event
                using (var e = GripInteraction.Get()); 
            }
            
        }

        if (IsGripping(devices[0]) && gripSession)
        {
            CheckHandPosition();
            boardInput = HandPosToBoardInput(handPosRel); 
        } else
        {
            if(gripSession) gripSession = false;
            boardInput = Vector3.zero;
        }

        if(handType == HandType.left)
        {
            using (var e = LeftBoardControlGripEvent.Get())
                e.leftGripInput.dir = boardInput;
        } else
        {
            using (var e = RightBoardControlGripEvent.Get())
                e.rightGripInput.dir = boardInput;
        }
        
    }

    /// <summary>
    /// Continues to check and send out event if either left or right hand is touching the interactable type "Water"
    /// </summary>
    private void WaterTouching()
    {
        bool isTouching = false;
        if (IsHandsTouching(Interactables.Water))
            isTouching = true;

        using (var e = WaveInteraction.Get()) 
            e.isTouching = isTouching;
    }

    /// <summary>
    /// Returns true if either the left or right hand is touching the given interactable type
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Bool value depending on if either hand is interacting with given type</returns>
    private bool IsHandsTouching(Interactables type)
    {
        return leftInteraction == type || rightInteraction == type;
    }

    /// <summary>
    /// Scales in percentage hand position values based on the starting coordinate, current coordinate and max height coordinate. 
    /// </summary>
    void CheckHandPosition()
    {
        if (startCoordinate != Vector3.zero)
        {
            // Assign variable values based on the direction the player is leaning / standing
            if (transform.localPosition.y > startCoordinate.y)
                handPosRel.y = Mathf.Clamp(Mathf.InverseLerp(startCoordinate.y, startCoordinate.y + maxUp, transform.localPosition.y), 0, maxUp); // Positive
            else
                handPosRel.y = Mathf.Clamp(-Mathf.InverseLerp(startCoordinate.y, startCoordinate.y + maxDown, transform.localPosition.y), maxDown, 0); // Negative
        }
    }
}
