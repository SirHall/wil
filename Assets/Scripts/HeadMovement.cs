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

    private enum MovementState { Stationary, Leaning, Warning, Fallen };

    [SerializeField]
    [Tooltip("Debug | Current movement state")]
    private MovementState movementState;

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

    void Start()
    {
        startCooridnate = cameraGameObject.transform.localPosition;
    }

    void Update()
    {
        currentCoordinate = cameraGameObject.transform.localPosition;

        CheckPlayerStability();

        using (var e = BoardControlEvent.Get())
            e.input.dir = HeadPosToBoardInput(headPosRel);
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
        if (headPos.x >= 0.4) { dir.y = 1.0f; } // Forward
        if (headPos.z >= 0.3) { dir.x = -1.0f; } //Left
        if (headPos.z <= -0.3) { dir.x = 1.0f; } //Right

        // Stationary
        if (headPos.x < 0.4) { dir.y = 0; } // Forward
        if (headPos.z < 0.3 && headPos.z > -0.3) { dir.x = 0; } // Side

        return dir;
    }


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

        // The scalar euclidean distance the head has moved from its original position 
        float headPosDist = Mathf.Max(Mathf.Abs(headPosRel.z), Mathf.Abs(headPosRel.x));

        if (headPosDist >= 1.0f) // Fallen criteria
            movementState = MovementState.Fallen;
        else if (headPosDist >= 0.7) // Warning criteria
            movementState = MovementState.Warning;
        else if (headPosDist >= 0.3f) // Leaning criteria
            movementState = MovementState.Leaning;
        else // Stationary criteria
            movementState = MovementState.Stationary;
    }
}
