using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameObject that contains the left hand controller")]
    GameObject vr_LeftHandGameObject;

    [SerializeField]
    [Tooltip("The GameObject that contains the right hand controller")]
    GameObject vr_RightHandGameObject;

    [SerializeField]
    [Tooltip("Starting local Cooridnate of the hands once gripped")]
    private Vector3 startCooridnate;

    /// <summary>
    /// The <see cref="GameObject"/> that contains the left hand
    /// </summary>
    public GameObject leftHandGameObject {
        get => vr_LeftHandGameObject;
        set => vr_LeftHandGameObject = value;
    }
    /// <summary>
    /// The <see cref="GameObject"/> that contains the right hand
    /// </summary>
    public GameObject rightHandGameObject {
        get => vr_RightHandGameObject;
        set => vr_RightHandGameObject = value;
    }

    private List<InputDevice> leftHandControllers = new List<InputDevice>();
    private List<InputDevice> rightHandControllers = new List<InputDevice>();

    private InputDevice leftHand;
    private InputDevice rightHand;

    private bool leftGrip;
    private bool rightGrip;
    // Start is called before the first frame update
    void Start() {
        InitialiseHands();
    }

    /// <summary>
    /// Set up the hands to contain a model an get animation if device exists
    /// </summary>
    private void InitialiseHands() {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, leftHandControllers);
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, rightHandControllers);

        if (leftHandControllers.Count > 0) leftHand = leftHandControllers[0];
        if (rightHandControllers.Count > 0) rightHand = rightHandControllers[0];
    }
    private bool LeftHandGripping() {
        // Left Grip condition
        bool gripCondition = false;
        if (leftHand.TryGetFeatureValue(CommonUsages.grip, out float gripvalue)) {
            if (gripvalue == 1)
                gripCondition = true;
        }
        return gripCondition;
    }
    private bool RightHandGripping() {
        // Right Grip condition
        bool gripCondition = false;
        if (rightHand.TryGetFeatureValue(CommonUsages.grip, out float gripvalue)) {
            if (gripvalue == 1)
                gripCondition = true;
        }
        return gripCondition;
    }
    // Update is called once per frame
    void Update()
    {
        if (!leftHand.isValid || !rightHand.isValid) {
            // Continue to initialise hand if not yet found
            InitialiseHands();
            return;
        }
        leftGrip = LeftHandGripping();
        rightGrip = RightHandGripping();

        print("Left Grip: " + leftGrip);
        print("Right Grip: " + rightGrip);
    }
}
