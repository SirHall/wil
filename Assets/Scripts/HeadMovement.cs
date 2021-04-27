using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameObject that contains the VR camera")]
    GameObject vr_CameraGameObject;

    [SerializeField]
    [Tooltip("Starting local Cooridnate of the player")]
    private Vector3 startCooridnate;

    [SerializeField]
    [Tooltip("Debug | current local cooridnates")]
    Vector3 currentCoordinate;

    [SerializeField]
    [Tooltip("Direction of the player")]
    public float left, right, forward, back;

    [SerializeField]
    [Tooltip("The max unit values the player can move in specified direction")]
    private float maxForward, maxBack, maxLeft, maxRight;

    private enum MovementState { Stationary, Leaning, Warning, Fallen };

    [SerializeField]
    [Tooltip("Debug | Current movement state")]
    private MovementState movementState;

    /// <summary>
    /// The <see cref="GameObject"/> that contains the camera, this is usually the "Head" of XR rigs.
    /// </summary>
    public GameObject cameraGameObject {
        get => vr_CameraGameObject;
        set => vr_CameraGameObject = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        startCooridnate = cameraGameObject.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        currentCoordinate = cameraGameObject.transform.localPosition;

        // Clear directional values
        forward = back = left = right = 0;

        // Reset movement State
        movementState = MovementState.Stationary;

        // Find difference in axis for current position compared to start position
        // If player leans forward his xdifference will be positive (Xdifference = 0.3f)
        float Xdifference = cameraGameObject.transform.localPosition.x - startCooridnate.x;
        float Zdifference = cameraGameObject.transform.localPosition.z - startCooridnate.z;

        // Assign variable values based on the direction the player is leaning / standing
        if (Xdifference > 0)
            forward = DirectionScale(Xdifference, maxForward); // Positive
        else
            back = DirectionScale(Xdifference, maxBack); // Negative

        if (Zdifference > 0)
            left = DirectionScale(Zdifference, maxLeft); // Positive
        else
            right = DirectionScale(Zdifference, maxRight); // Negative


        // Set movement state based on the most urgent action. 
        // Ruff implementation currently used for debug to easily identify player state
        float[] directions = { forward, back, left, right };
        int priority = 100;
        foreach (float direction in directions) 
        {
            // Fallen Criteria
            if (direction == 1) 
            {
                movementState = MovementState.Fallen;
                priority = 1;
                break;
            }
            // Warning Criteria
            else if (checkBetween(direction, 0.7f, 1) && priority > 1) {
                movementState = MovementState.Warning;
                priority = 2;
            }
            // Leaning Criteria
            else if (checkBetween(direction, 0.3f, 0.7f) && priority > 2) {
                movementState = MovementState.Leaning;
                priority = 3;
            }
            // Stationary Criteria
            else if (checkBetween(direction, 0f, 0.3f) && priority > 3) {
                movementState = MovementState.Stationary;
                priority = 4;
            }
        }
    }
    /// <summary>
    /// Return true if a number is between a min and max value
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns>Bool value based on if a number is between a min and max value</returns>
    private bool checkBetween(float direction, float minValue, float maxValue) 
    {
        return direction >= minValue && direction < maxValue;
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
}
