using System.Collections;
using System.Collections.Generic;
using Excessives.Unity;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardController : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor motor;
    public KinematicCharacterMotor Motor { get => motor; }
    [SerializeField] float sensitivity = 1.0f;

    CharacterInput input = new CharacterInput();
    Vector2 cameraRot = Vector2.zero;

    [SerializeField] InputAction cardinalInput;
    Vector2 Cardinal { get => cardinalInput.ReadValue<Vector2>(); }

    [SerializeField] float waveAccel = 20.0f;

    [SerializeField] float moveAccel = 10.0f;
    [SerializeField] float rotateAccel = 15.0f;
    [SerializeField] float drag = 0.1f;
    [SerializeField] float waveGravity = 15.0f;
    [SerializeField] float wavePullUpAccel = 8.0f;
    [SerializeField] float waveForwardAccel = 20f;

    void Awake()
    {
        motor.CharacterController = this;
    }

    void OnEnable()
    {
        cardinalInput.Enable();
    }

    void Start() { }

    void Update()
    {
        input.dir = Cardinal;
    }

    void OnDisable()
    {
        cardinalInput.Disable();
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
        currentRotation *= Quaternion.AngleAxis(input.dir.x * rotateAccel * deltaTime, transform.up);
    }

    void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // Drag is only applied to horizontal components
        currentVelocity = (currentVelocity * (1f / (1f + (drag * deltaTime)))).WithY(currentVelocity.y);

        if (motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity += transform.forward * input.dir.y * moveAccel * deltaTime;

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
            Debug.DrawRay(transform.position, Vector3.up, Color.blue, 0.1f, false);
            Debug.DrawRay(transform.position, motor.GroundingStatus.GroundNormal, Color.green, 0.1f, false);
            Debug.DrawRay(transform.position, wallGravity, Color.red, 0.1f, false);
        }
    }

    #endregion
}

struct CharacterInput
{
    public Vector2 dir;
    // public bool jump;
}