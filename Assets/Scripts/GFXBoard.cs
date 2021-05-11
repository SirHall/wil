using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GFXBoard : MonoBehaviour
{


    [TitleGroup("References")] [SerializeField] BoardController board;
    [TitleGroup("References")] [SerializeField] Rigidbody rb;

    // [Title("Animation")]
    [Tooltip("The velocity of the board at which the sway animation will stop entirely")]
    [SerializeField] float animUpperStopVel = 5.0f;

    [TitleGroup("Position Animation Offset")]
    [SerializeField] Vector3 bobPosTimeScale = Vector3.one;
    [TitleGroup("Position Animation Offset")]
    [SerializeField] Vector3 bobPosMaxOffset = Vector3.one;

    [TitleGroup("Rotation Animation Offset")]
    [SerializeField] Vector3 bobRotTimeScale = Vector3.one;
    [TitleGroup("Rotation Animation Offset")]
    [SerializeField] Vector3 bobRotMaxDegreeOffset = new Vector3(5.0f, 5.0f, 5.0f);

    [TitleGroup("Follow")] [SerializeField] float rotateLerpSpeed = 10.0f;
    [TitleGroup("Follow")] [SerializeField] float positionLerpSpeed = 10.0f;


    void Start()
    {

    }

    Quaternion animRot = Quaternion.identity;
    Vector3 animPos = Vector3.zero;

    void FixedUpdate()
    {
        //Temporarily undo the last tick's animation offsets so it doesn't affect our base lerp
        transform.localRotation *= Quaternion.Inverse(animRot);
        transform.localPosition -= animPos;

        transform.position = board.Motor.TransientPosition;
        transform.rotation = board.Motor.TransientRotation;

        // transform.position = Vector3.Lerp(transform.position, board.transform.position, Time.deltaTime * positionLerpSpeed);
        // transform.rotation = Quaternion.Lerp(
        //     transform.rotation,
        //     board.transform.rotation,
        //     // board.Motor.GetDirectionTangentToSurface(board.transform.forward, Vector3.up),
        //     Time.deltaTime * rotateLerpSpeed);

        // Reduce the scale of the animation as we go faster
        float animScale = Mathf.Clamp01(1.0f /
            Mathf.Clamp(board.Motor.Velocity.magnitude / animUpperStopVel, 1.0f, float.MaxValue));

        animRot = Quaternion.Euler(
            Mathf.Sin(Time.time * bobRotTimeScale.x) * bobRotMaxDegreeOffset.x * animScale,
            Mathf.Sin(Time.time * bobRotTimeScale.y) * bobRotMaxDegreeOffset.y * animScale,
            Mathf.Sin(Time.time * bobRotTimeScale.z) * bobRotMaxDegreeOffset.z * animScale
        );

        animPos = new Vector3(
            Mathf.Sin(Time.time * bobPosTimeScale.x) * bobPosMaxOffset.x * animScale,
            Mathf.Sin(Time.time * bobPosTimeScale.y) * bobPosMaxOffset.y * animScale,
            Mathf.Sin(Time.time * bobPosTimeScale.z) * bobPosMaxOffset.z * animScale
        );

        transform.localRotation *= animRot;
        transform.localPosition += animPos;
    }
}
