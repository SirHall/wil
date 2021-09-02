using System.Collections;
using UnityConstantsGenerator;
using System.Collections.Generic;
using Excessives.Unity;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class BoardController : MonoBehaviour, ICharacterController
{

    public static BoardController Instance { get; private set; }

    [SerializeField] KinematicCharacterMotor motor;
    public KinematicCharacterMotor Motor { get => motor; }

    BoardInput input = new BoardInput();
    Vector2 cameraRot = Vector2.zero;

    private bool isIntroStarted;

    [SerializeField] float rotateAccel = 15.0f;
    [Tooltip("Drag applied to the board")]
    [SerializeField] float drag = 0.1f;
    [Tooltip("Gravity applied to the board pulling it down")]
    [SerializeField] float waveGravity = 15.0f;
    [Tooltip("Force applied to the board pulling it upwards when parallel to the barrel")]
    [SerializeField] float wavePullUpAccel = 8.0f;
    [Tooltip("As the board angles downwards this forward force is applied")]
    [SerializeField] float waveForwardAccel = 20f;
    [Tooltip("How far from being parallel to the barrel the board must be for the player to fall, 1 = parallel, 0 = perpendicular")]
    [SerializeField] [Range(0.0f, 1.0f)] float boardDirDotFallThreshold = 0.5f;
    [Tooltip("When the incline below the player goes above this angle, the player will fall off")]
    [SerializeField] [Range(0.0f, 90.0f)] float maxGroundAngle = 60.0f;

    [SerializeField] bool introEnabled = true;
    [SerializeField] [ShowIfGroup("introEnabled")] [FoldoutGroup("introEnabled/Intro")] Transform introStartPos;
    [SerializeField] [ShowIfGroup("introEnabled")] [FoldoutGroup("introEnabled/Intro")] Transform introEndPos;
    [SerializeField] [ShowIfGroup("introEnabled")] [FoldoutGroup("introEnabled/Intro")] float introTime;

    /// <summary>
    /// Is this board accepting user input?
    /// </summary>
    public bool InputAccepted { get => inputAccepted; set => inputAccepted = value; }
    [SerializeField] bool inputAccepted = true;

    /// <summary>
    /// Stops the board from moving along cardinal axis'
    /// </summary>
    public void StopImmediately() => immediateStop = true;
    bool immediateStop = false;

    void Awake()
    {
        motor.CharacterController = this;
    }

    void Start()
    {
        if (introEnabled)
        {
            isIntroStarted = false;
            inputAccepted = false;
            Vector3 dir = introEndPos.position - introStartPos.position;
            motor.SetPositionAndRotation(introStartPos.position, Quaternion.LookRotation(dir, Vector3.up));
        }
    }

    void OnEnable()
    {
        BoardControlEvent.RegisterListener(OnBoardControlEvent);

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDisable()
    {
        BoardControlEvent.UnregisterListener(OnBoardControlEvent);

        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (WaveScore.IsPlaying && !WaveScore.IsWarmup && !isIntroStarted)
            StartCoroutine(Intro());

        if (!WaveScore.IsPlaying)
            input.dir = Vector2.zero;

        if (WaveScore.IsPlaying && !WaveScore.IsWarmup)
            CheckBoardStability();
    }

    // A controller has announced new data
    void OnBoardControlEvent(BoardControlEvent e)
    {
        if (WaveScore.IsPlaying)
            input = e.input;
    }

    IEnumerator Intro()
    {

        isIntroStarted = true;
        // Only run the intro if it is enabled, and all positions are set
        introEnabled = introEnabled && introStartPos != null && introEndPos != null;


        if (introEnabled)
        {
            Vector3 dir = introEndPos.position - introStartPos.position;
            float introVel = dir.magnitude / introTime;

            float introClock = 0.0f;

            // Allow one frame to pass so the above SetPositionAndRotation takes effect
            yield return null;

            while (introClock <= introTime)
            {
                introClock += Time.deltaTime;
                // float dist = Mathf.Min(dir.magnitude, introVel * Time.deltaTime);
                // motor.pos(motor.TransientPosition + dir.normalized * dist);
                // dir = introEndPos.position - motor.TransientPosition;
                Motor.BaseVelocity = (introEndPos.position - introStartPos.position) / introTime;
                yield return null;
            }

            Motor.BaseVelocity += (introEndPos.position - introStartPos.position) / introTime;

            inputAccepted = true;

        }
    }

    #region ICharacterController

    void ICharacterController.AfterCharacterUpdate(float deltaTime) { }

    void ICharacterController.BeforeCharacterUpdate(float deltaTime) { }

    bool ICharacterController.IsColliderValidForCollisions(Collider coll) =>
        (coll.gameObject.layer == (int)LayerId.Water) ||
        (coll.gameObject.layer == (int)LayerId.Default);

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
        currentRotation *= Quaternion.AngleAxis((input.dir.x * 2.4f) * rotateAccel * deltaTime, transform.up);
    }

    void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (immediateStop == true)
        {
            immediateStop = false;
            currentVelocity = currentVelocity.WithX(0.0f).WithZ(0.0f);
        }

        if (!inputAccepted)
            return;
        // Drag is only applied to horizontal components
        currentVelocity = (currentVelocity * (1f / (1f + (drag * deltaTime)))).WithY(currentVelocity.y);

        if (motor.GroundingStatus.IsStableOnGround)
        {
            Collider col = motor.GroundingStatus.GroundCollider;

            if (col.name == "WavePart") // Wave force
            {
                // TODO: Leaning forward/backward increases speed/drag
                currentVelocity +=
                   Vector3.ProjectOnPlane(
                       (
                            (col.transform.forward * -wavePullUpAccel) +
                            Vector3.Lerp(
                                col.transform.right * ((BarrelSettings.Instance.SurfDir == RightLeft.Right) ? 1.0f : -1.0f),
                                Motor.CharacterForward,
                                0.5f) * waveForwardAccel
                       ) * deltaTime,
                       Vector3.up
                   );
            }
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

    // This will check that the board is not too high up the wave, and that it
    // is not *too* perpendicular to the wave force direction,
    // otherwise it will trigger the GameLost event.
    void CheckBoardStability()
    {
        // Wave goes along x axis, use dot product to find whether we are parallel or perpedicular to the x axis
        float dirDot = Mathf.Abs(Vector3.Dot(Vector3.right, Motor.CharacterForward));
        if (dirDot < boardDirDotFallThreshold)
            using (var e = GameLost.Get())
                e.cause = "Board perpendicular to barrel direction";

        // Ensure that the board's ground normal isn't too steep
        // Easiest way to do this is to find the angle between the normal and the up vector
        float groundAngle = Vector3.Angle(Vector3.up, Motor.GroundingStatus.GroundNormal);
        if (groundAngle > maxGroundAngle)
            using (var e = GameLost.Get())
                e.cause = "Water beneath board too steep";
    }

    #endregion
}