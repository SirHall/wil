using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Excessives.Unity;

public class GFXBoard : MonoBehaviour
{
    public static GFXBoard Instance { get; private set; }

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

    [SerializeField] float posInterp = 3.0f;
    [SerializeField] float rotInterp = 3.0f;

    [Tooltip("Toggle if the surfboard is bobbing on the water or stationary")]
    private bool isBobbing;

    BoardController Board => BoardController.Instance;

    public static float BoardHeight { get; private set; }

    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There may only be one instance of SurfPoint at any time");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GameSettingsEvent.RegisterListener(OnGameplaySettingEvent);
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
            GameSettingsEvent.UnregisterListener(OnGameplaySettingEvent);
        }
    }


    void LateUpdate()
    {
        if (!WaveScore.IsPlaying) isBobbing = false;

        if (Board is null)
            return;

        // Make the board track the player character
        transform.position = Board.Motor.TransientPosition;
        transform.rotation = Board.Motor.TransientRotation;

        Vector3 forwardBob = waterBobForwardPoint.position.WithY(v => WaterData.Instance.EvalAtWorldPos(v));
        Vector3 rearLeftBob = waterBobRearLeftPoint.position.WithY(v => WaterData.Instance.EvalAtWorldPos(v));
        Vector3 rearRighhtBob = waterBobRearRightPoint.position.WithY(v => WaterData.Instance.EvalAtWorldPos(v));

        // Position/rotation due to wave bobbing
        Vector3 bobNormal = -UnityExcessives.FindNormal(forwardBob, rearLeftBob, rearRighhtBob);
        Vector3 bobPos = UnityExcessives.MeanPos(forwardBob, rearLeftBob, rearRighhtBob);

        // Position/rotation from floor/barrel
        Vector3 motorNormal = Board.Motor.GroundingStatus.GroundNormal;
        Vector3 motorPos = Board.Motor.TransientPosition;

        // We select between using bobbing information versus motor info, based on which water level is higher
        bool useMotor = motorPos.y > bobPos.y;

        Vector3 normal = useMotor ? motorNormal : bobNormal;
        Vector3 pos = useMotor ? motorPos : bobPos;

        if (isBobbing)
        {
            transform.position = Vector3.Lerp(transform.position, pos, 1.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Board.Motor.CharacterForward, normal), 1.0f);
        }
    }

    void OnGameplaySettingEvent(GameSettingsEvent e)
    {
        isBobbing = e.gameplaySettings.bobbing;
    }
}
