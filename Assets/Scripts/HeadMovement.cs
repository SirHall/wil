using System.Collections;
using System.Collections.Generic;
using Excessives;
using Excessives.LinqE;
using Excessives.Unity;
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
    [Tooltip("The max unit values the player can move in specified direction")]
    private float maxForward, maxBack, maxLeft, maxRight;

    [SerializeField]
    [Tooltip("Gameobjects which define the coordinates for the max boundary values")]
    private GameObject forwardBoundary, backBoundary, leftBoundary, rightBoundary;

    [SerializeField]
    [Tooltip("Debug | Current movement state")]
    private MovementState movementState;

    [SerializeField]
    [Tooltip("Check if the player has been scored on current state")]
    private bool isScored;

    [SerializeField]
    [Tooltip("Total number of times the player has entered the warning state")]
    private int totalWarnings;

    [SerializeField]
    [Tooltip("Time in seconds the player has remained in warning state")]
    private float timeInWarning;

    #region Bookkeeping

    /// <summary>
    /// The player's head position relative to the starting head position
    /// </summary>
    Vector3 headPosRel = Vector3.zero;

    #endregion

    /// <summary>
    /// The <see cref="GameObject"/> that contains the camera, this is usually the "Head" of XR rigs.
    /// </summary>
    public GameObject cameraGameObject
    {
        get => vr_CameraGameObject;
        set => vr_CameraGameObject = value;
    }
    private void Awake()
    {
        // Assign MaxValues based on boundry gameobjects coordinates
        maxForward = Mathf.Abs(Mathf.Round(forwardBoundary.transform.localPosition.z * 100f) / 100f);
        maxBack = Mathf.Abs(Mathf.Round(backBoundary.transform.localPosition.z * 100f) / 100f);
        maxLeft = Mathf.Abs(Mathf.Round(leftBoundary.transform.localPosition.x * 100f) / 100f);
        maxRight = Mathf.Abs(Mathf.Round(rightBoundary.transform.localPosition.x * 100f) / 100f);
    }
    void Start()
    {
        startCooridnate = cameraGameObject.transform.localPosition;
    }

    void Update()
    {
        currentCoordinate = cameraGameObject.transform.localPosition;
        CheckPlayerStability();
        SetState();
        HeadScoring();
        using (var e = BoardControlEvent.Get())
            e.input.dir = HeadPosToBoardInput(headPosRel);

        using (var e = VisualControlEvent.Get())
            e.dir = headPosRel;

        using (var e = ScoreControlEvent.Get())
        {
            e.warningAmt = totalWarnings;
            e.warningTime = timeInWarning;
        }

    }

    /// <summary>
    /// Return true if a number is between a min and max value
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns>Bool value based on if a number is between a min and max value</returns>
    private bool checkBetween(float direction, float minValue, float maxValue) =>
        direction >= minValue && direction < maxValue;

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


    private Vector3 HeadPosToBoardInput(Vector3 headPos)
    {
        Vector3 dir = Vector3.zero;

        // Apply forward movement if the player leans 40% or greater based on the max forward value.
        // and use 30% on the sides

        // headPos X axis == dir Y axis (Forward)
        // headPos Z axis == dir X Axis (Side)

        // Leaning
        if (headPos.x >= 0.4) { dir.y = headPos.x; } // Forward
        if (headPos.z >= 0.3) { dir.x = -headPos.z; } //Left
        if (headPos.z <= -0.3) { dir.x = Mathf.Abs(headPos.z); } //Right

        // Stationary
        if (headPos.x < 0.4) { dir.y = 0; } // Forward
        if (headPos.z < 0.3 && headPos.z > -0.3) { dir.x = 0; } // Side

        return dir;
    }

    /// <summary>
    /// Manages head position values based on current coordinates from start coordinates
    /// </summary>
    void CheckPlayerStability()
    {
        // Diff X axis = Forward
        // Diff Z Axis = Side
        Vector3 diff = cameraGameObject.transform.localPosition - startCooridnate;

        // Assign variable values based on the direction the player is leaning / standing
        if (diff.x > 0)
            headPosRel.x = DirectionScale(diff.x, maxForward); // Positive
        else
            headPosRel.x = -DirectionScale(diff.x, maxBack); // Negative

        if (diff.z > 0)
            headPosRel.z = DirectionScale(diff.z, maxLeft); // Positive
        else
            headPosRel.z = -DirectionScale(diff.z, maxRight); // Negative
    }

    // This function takes in the head tilt and transforms it into a movement
    // state, this allows code anywhere to check the board's stability
    public static MovementState HeadTiltToState(Vector3 headTilt)
    {
        // The scalar euclidean distance the head has moved from its original position 
        float headPosDist = Mathf.Max(Mathf.Abs(headTilt.z), Mathf.Abs(headTilt.x));

        if (headPosDist >= 1.0f) // Fallen criteria
            return MovementState.Fallen;
        else if (headPosDist >= 0.7) // Warning criteria
            return MovementState.Warning;
        else if (headPosDist >= 0.3f) // Leaning criteria
            return MovementState.Leaning;
        else // Stationary criteria
            return MovementState.Stationary;

    }

    /// <summary>
    /// Sets the movement state of the player based on the heads distance from the center of the board
    /// </summary>
    void SetState()
    {
        MovementState previousState = movementState;

        movementState = HeadTiltToState(headPosRel);

        // Reset isScored if movement state has changed
        if (previousState != movementState)
            isScored = false;
    }
    /// <summary>
    /// Sets scoring values based on head movement conditions
    /// </summary>
    void HeadScoring()
    {
        switch (movementState)
        {
            case MovementState.Warning:
                // Check if warning state has already been scored
                if (!isScored)
                {
                    totalWarnings += 1;
                    isScored = true;
                }
                timeInWarning += Time.deltaTime;
                break;
        }
    }
}

public enum MovementState
{
    Stationary,
    Leaning,
    Warning,
    Fallen
}