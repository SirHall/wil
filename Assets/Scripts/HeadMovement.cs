using System.Collections;
using System.Collections.Generic;
using Excessives;
using Excessives.LinqE;
using Excessives.Unity;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Starting local Cooridnate of the player")]
    private Vector3 startCoordinate;

    [Tooltip("The remaining and primary active camera in the scene hierarchy")]
    private GameObject mainCamera;

    [SerializeField]
    [Tooltip("Debug | current local cooridnates")]
    private Vector3 currentCoordinate;

    [Tooltip("The max unit values the player can move in specified direction")]
    private float maxForward, maxBack, maxLeft, maxRight;

    [SerializeField]
    [Tooltip("Gameobjects which define the coordinates for the max boundary values")]
    private GameObject forwardBoundary, backBoundary, leftBoundary, rightBoundary;

    [SerializeField]
    [Tooltip("Debug | Current movement state")]
    private MovementState movementState;

    [SerializeField]
    [Tooltip("Debug | Current position state")]
    private PositionState positionState;

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
    [SerializeField]
    private Vector3 headPosRel = Vector3.zero;

    #endregion
    private void Start()
    {
        // Assign MaxValues based on boundry gameobjects coordinates
        maxForward = Mathf.Abs(Mathf.Round(forwardBoundary.transform.localPosition.z * 100f) / 100f);
        maxBack = Mathf.Abs(Mathf.Round(backBoundary.transform.localPosition.z * 100f) / 100f);
        maxLeft = Mathf.Abs(Mathf.Round(leftBoundary.transform.localPosition.x * 100f) / 100f);
        maxRight = Mathf.Abs(Mathf.Round(rightBoundary.transform.localPosition.x * 100f) / 100f);
    }

    void Update()
    {
        if (mainCamera != Camera.main.gameObject)
        {
            startCoordinate = Camera.main.transform.localPosition;
            mainCamera = Camera.main.gameObject;
        }
        if (mainCamera == null)
        {
            Debug.LogError("No active camera found in the scene");
            return;
        }

        //Debug
        currentCoordinate = mainCamera.transform.localPosition;

        CheckStartingPos();
        CheckPlayerStability();
        SetState();
        HeadScoring();
        CallGlobalEvents();

        // Check if head's moved to far, if so then trigger the GameLost event

        MovementState moveState = HeadMovement.HeadTiltToState(headPosRel.WithY(mainCamera.transform.position.y));
        if (moveState == MovementState.Fallen)
            using (var e = GameLost.Get())
                e.cause = "Leaned head too far";

    }

    /// <summary>
    /// Re-assign start coordinate variable if it's zero
    /// </summary>
    private void CheckStartingPos()
    {
        if (startCoordinate == Vector3.zero)
            startCoordinate = Camera.main.transform.localPosition;
    }

    private void CallGlobalEvents()
    {
        using (var e = BoardControlEvent.Get())
            e.input.dir = HeadPosToBoardInput(headPosRel);

        using (var e = ScoreControlEvent.Get())
        {
            e.warningAmt = totalWarnings;
            e.warningTime = timeInWarning;
        }

        using (var e = VisualControlEvent.Get())
        {
            e.dir = headPosRel;
            e.dir.y = mainCamera.transform.position.y;
        }

        using (var e = SoundControlEvent.Get())
        {
            e.headInput.dir = headPosRel;
            e.headInput.dir.y = mainCamera.transform.position.y;
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
        float scaledValue;
        scaledValue = Mathf.Clamp(Mathf.Abs(value), 0, maxValue) / maxValue;

        if (value < 0)
            scaledValue = -scaledValue;

        return scaledValue;
    }

    /// <summary>
    /// Gets the current head position and coverts its coordinates to the board controllers coordinates so the board can be rotated / moved. 
    /// </summary>
    /// <param name="headPos"></param>
    /// <returns>Vector 3 coordinate values which match with the board controllers coordinate values</returns>
    private Vector3 HeadPosToBoardInput(Vector3 headPos)
    {
        Vector3 dir = Vector3.zero;

        // headPos Z axis == dir Y axis (Forward)
        // headPos X axis == dir X Axis (Side)

        // Cutoff values determin when the head position values should be ignored and returned as a 0 value (Essentially making the surfboard stationary)
        // Or when they should return their true values which will be used by the surfboard to move it. 
        float forwardCutoff = 0.2f;
        float sidewaysCutoff = 0.1f;

        // Leaning
        if (headPos.z >= forwardCutoff) { dir.y = headPos.z - forwardCutoff; } // Forward
        if (headPos.x >= sidewaysCutoff) { dir.x = headPos.x - sidewaysCutoff; } //Right
        if (headPos.x <= -sidewaysCutoff) { dir.x = headPos.x + sidewaysCutoff; } //Left

        // Stationary
        if (headPos.z < forwardCutoff) { dir.y = 0; } // Forward
        if (headPos.x < sidewaysCutoff && headPos.x > -sidewaysCutoff) { dir.x = 0; } // Side

        return dir;
    }

    /// <summary>
    /// Manages head position values based on current coordinates from start coordinates
    /// </summary>
    void CheckPlayerStability()
    {
        Vector3 diff = mainCamera.transform.localPosition - startCoordinate;

        float forwardPos = diff.z; // Forward, Back
        float sidePos = diff.x; // Left, Right

        // Assign variable values based on the direction the player is leaning / standing
        headPosRel.z = DirectionScale(forwardPos, maxForward); // Positive
        headPosRel.x = DirectionScale(sidePos, maxLeft); // Positive
    }

    /// <summary>
    /// Takes in the head tilt and returns a movement state
    /// </summary>
    /// <param name="headTilt"></param>
    /// <returns>Enum movement state depending on head tilt</returns>
    public static MovementState HeadTiltToState(Vector3 headTilt)
    {
        // The scalar euclidean distance the head has moved from its original position 
        float headPosDist = Mathf.Max(Mathf.Abs(headTilt.x), Mathf.Abs(headTilt.z));

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

public enum PositionState
{
    Prone, // Lying on stomach
    Standing, // In a good hunched standing position
    Extended // Standing tall and not hunching over
}