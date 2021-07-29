using System.Collections;
using System.Collections.Generic;
using Excessives.Unity;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class BoardController : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor motor;
    public KinematicCharacterMotor Motor { get => motor; }
    [SerializeField] float sensitivity = 1.0f;

    BoardInput input = new BoardInput();
    Vector2 cameraRot = Vector2.zero;

    [SerializeField] float waveAccel = 20.0f;

    [SerializeField] float moveAccel = 10.0f;
    [SerializeField] float rotateAccel = 15.0f;
    [SerializeField] float drag = 0.1f;
    [SerializeField] float waveGravity = 15.0f;
    [SerializeField] float wavePullUpAccel = 8.0f;
    [SerializeField] float waveForwardAccel = 20f;


    [SerializeField] bool introEnabled = true;
    [SerializeField] [ShowIfGroup("introEnabled")] [FoldoutGroup("introEnabled/Intro")] Transform introStartPos;
    [SerializeField] [ShowIfGroup("introEnabled")] [FoldoutGroup("introEnabled/Intro")] Transform introEndPos;
    [SerializeField] [ShowIfGroup("introEnabled")] [FoldoutGroup("introEnabled/Intro")] float introTime;

    /// <summary>
    /// Is this board accepting user input?
    /// </summary>
    bool inputAccepted = true;

    void Awake()
    {
        motor.CharacterController = this;
    }

    void Start()
    {
        if (introEnabled)
            StartCoroutine(Intro());
    }

    void OnEnable()
    {
        BoardControlEvent.RegisterListener(OnBoardControlEvent);
    }

    void OnDisable()
    {
        BoardControlEvent.UnregisterListener(OnBoardControlEvent);
    }

    void Update()
    {
        if (!WaveScore.IsPlaying)
            input.dir = Vector2.zero;
    }

    // A controller has announced new data
    void OnBoardControlEvent(BoardControlEvent e)
    {
        if (WaveScore.IsPlaying)
            input = e.input;
    }

    IEnumerator Intro()
    {
        inputAccepted = false;

        // Only run the intro if it is enabled, and all positions are set
        introEnabled = introEnabled && introStartPos != null && introEndPos != null;

        if (introEnabled)
        {
            Vector3 dir = introEndPos.position - introStartPos.position;
            float introVel = dir.magnitude / introTime;
            motor.SetPositionAndRotation(introStartPos.position, Quaternion.LookRotation(dir, Vector3.up));

            // Allow one frame to pass so the above SetPositionAndRotation takes effect
            yield return null;

            while (dir.magnitude > 0.01f)
            {
                float dist = Mathf.Min(dir.magnitude, introVel * Time.deltaTime);
                motor.SetPosition(motor.TransientPosition + dir.normalized * dist);
                dir = introEndPos.position - motor.TransientPosition;
                yield return null;
            }

            inputAccepted = true;
        }
    }

    #region ICharacterController

    void ICharacterController.AfterCharacterUpdate(float deltaTime) { }

    void ICharacterController.BeforeCharacterUpdate(float deltaTime) { }

    bool ICharacterController.IsColliderValidForCollisions(Collider coll) { return true; }

    void ICharacterController.OnDiscreteCollisionDetected(Collider hitCollider) { }

    void ICharacterController.OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    void ICharacterController.OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    void ICharacterController.PostGroundingUpdate(float deltaTime) { }

    void ICharacterController.ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    void ICharacterController.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (!inputAccepted)
            return;
        // Inputs direction X value is multiplied to make greater head movements apply a large value in comparison to smaller values. 
        currentRotation *= Quaternion.AngleAxis((input.dir.x * 3f) * rotateAccel * deltaTime, transform.up);
    }

    void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (!inputAccepted)
            return;
        // Drag is only applied to horizontal components
        currentVelocity = (currentVelocity * (1f / (1f + (drag * deltaTime)))).WithY(currentVelocity.y);

        if (motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity += transform.forward * (input.dir.y * 1.1f) * moveAccel * deltaTime;

            Collider col = motor.GroundingStatus.GroundCollider;

            if (col.name == "WavePart") // Wave force
                currentVelocity +=
                    Vector3.ProjectOnPlane(
                        (
                            (col.transform.forward * -wavePullUpAccel) +
                            (col.transform.right * waveForwardAccel)
                        ) * deltaTime,
                        Vector3.up
                    );
        }
        else // We're in the air
        {
            // Gravity
            currentVelocity += Vector3.up * -9.81f * deltaTime;
        }

        if (motor.GroundingStatus.FoundAnyGround)
        {
            // This allows us to slide down any inclination
            Vector3 wallGravity = Vector3.ProjectOnPlane(motor.GroundingStatus.GroundNormal, Vector3.up) * 15f * deltaTime;
            currentVelocity += wallGravity;
            // Debug.DrawRay(transform.position, Vector3.up, Color.blue, 0.1f, false);
            // Debug.DrawRay(transform.position, motor.GroundingStatus.GroundNormal, Color.green, 0.1f, false);
            // Debug.DrawRay(transform.position, wallGravity, Color.red, 0.1f, false);
        }
    }

    #endregion
}