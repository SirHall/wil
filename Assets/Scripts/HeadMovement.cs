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
        maxForward = forwardBoundary.transform.localPosition.z;
        maxBack = backBoundary.transform.localPosition.z;
        maxLeft = leftBoundary.transform.localPosition.x;
        maxRight = rightBoundary.transform.localPosition.x;
    }

    void Update()
    {
        if (Camera.main is null)
            return;

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
        ScalePlayerStability();
        SetState();
        HeadScoring();
        CallGlobalEvents();

        // Check if head's moved to far, if so then trigger the GameLost event
        MovementState moveState = HeadMovement.HeadMovementState(headPosRel.WithY(mainCamera.transform.position.y));
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
    /// Scales head coordinates to between -1 to 0 (Left, Back) OR 0 to 1 (Right, Forward) depending on percentage away from starting position to boundary
    /// </summary>
    void ScalePlayerStability()
    {
        float forwardPos = mainCamera.transform.localPosition.z; // Forward, Back
        float sidePos = mainCamera.transform.localPosition.x; // Left, Right
        float heightPos = mainCamera.transform.localPosition.y; // Up, Down

        // Assign variable values based on the direction the player is leaning / standing
        if (forwardPos > 0)
            headPosRel.z = Mathf.InverseLerp(0, maxForward, forwardPos); // Positive
        else
            headPosRel.z = -Mathf.InverseLerp(0, maxBack, forwardPos); // Negative

        if (sidePos > 0)
            headPosRel.x = Mathf.InverseLerp(0, maxRight, sidePos); // Positive
        else
            headPosRel.x = -Mathf.InverseLerp(0, maxLeft, sidePos); // Negative

        headPosRel.y = Mathf.InverseLerp(0, startCoordinate.y, heightPos);
    }

    /// <summary>
    /// Takes in the head tilt and returns a movement state
    /// </summary>
    /// <param name="headLean"></param>
    /// <returns>Enum movement state depending on head tilt</returns>
    public static MovementState HeadMovementState(Vector3 headLean)
    {
        // The scalar euclidean distance the head has moved from its original position 
        float headPosDist = Mathf.Max(Mathf.Abs(headLean.x), Mathf.Abs(headLean.z));

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
    /// Takes in the head tilt and returns a movement state
    /// </summary>
    /// <param name="headTilt"></param>
    /// <returns>Enum movement state depending on head tilt</returns>
    public static PositionState HeadPositionState(Vector3 headHeight)
    {
        // The scalar euclidean distance the head has moved from its original position 

        if (headHeight.y <= 0.3f) // Fallen criteria
            return PositionState.Prone;
        else if (headHeight.y <= 0.8) // Warning criteria
            return PositionState.Crouched;
        else
            return PositionState.Standing;

    }
    /// <summary>
    /// Sets the movement state of the player based on the heads distance from the center of the board
    /// </summary>
    void SetState()
    {
        MovementState previousState = movementState;

        movementState = HeadMovementState(headPosRel);
        positionState = HeadPositionState(headPosRel);

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
                    WaveScore.WarningAmt += 1;
                    isScored = true;
                }
                WaveScore.WarningTime += Time.deltaTime;
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
    Crouched, // In a good riding position
    Standing // Standing tall and not hunching over
}