using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Excessives.Unity;

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

    Quaternion animRot = Quaternion.identity;
    Vector3 animPos = Vector3.zero;

    [SerializeField] [FoldoutGroup("WaterBobPoints")] Transform waterBobForwardPoint;
    [SerializeField] [FoldoutGroup("WaterBobPoints")] Transform waterBobRearRightPoint;
    [SerializeField] [FoldoutGroup("WaterBobPoints")] Transform waterBobRearLeftPoint;

    void FixedUpdate()
    {
        //Temporarily undo the last tick's animation offsets so it doesn't affect our base lerp
        transform.localRotation *= Quaternion.Inverse(animRot);
        transform.localPosition -= animPos;

        // transform.position = board.Motor.TransientPosition;
        // transform.rotation = board.Motor.TransientRotation;

        transform.position = Vector3.Lerp(transform.position, board.transform.position, Time.deltaTime * positionLerpSpeed);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            board.transform.rotation,
            // Quaternion.FromToRotation(
            //     board.transform.forward,
            //     board.Motor.GetDirectionTangentToSurface(board.transform.forward, Vector3.up)
            // ),
            Time.deltaTime * rotateLerpSpeed);

        Vector3 bobNormal = -UnityExcessives.FindNormal(waterBobForwardPoint.position, waterBobRearLeftPoint.position, waterBobRearRightPoint.position);
        Vector3 bobPos = UnityExcessives.MeanPos(waterBobForwardPoint.position, waterBobRearLeftPoint.position, waterBobRearRightPoint.position);

        animPos = Vector3.Lerp(animPos, Vector3.up * WaterData.Instance.EvalAtWorldPos(bobPos), 1.0f);

        animRot = Quaternion.FromToRotation(Vector3.up, bobNormal);

        transform.localRotation *= animRot;
        transform.localPosition += animPos;
    }
}
