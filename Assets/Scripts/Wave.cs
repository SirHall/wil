using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives;
using Excessives.LinqE;
using Excessives.Unity;
using Sirenix.OdinInspector;
using KinematicCharacterController;
using static UnityEngine.ParticleSystem;

//         ______________________________________
//        |                 NOTE                 |
//        |THE BELOW USES RADIANS FOR ALL ANGLES!|
//        |    Except where otherwise stated     |
//         ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾

public class Wave : MonoBehaviour, IMoverController
{
    [SerializeField] PhysicsMover mover;

    [SerializeField] GameObject wavePartPrefab;
    [Tooltip("Total number of wave parts that are instantiated in the wavepart pool")]
    [SerializeField] int wavePartCount = 100;

    [Tooltip("The assumed length of the base wavepart")]
    [SerializeField] float wavePartLength = 1.0f;
    [Tooltip("The assumed width of the base wavepart")]
    [SerializeField] float wavePartWidth = 1.0f;

    List<GameObject> waveParts = new List<GameObject>();

    [Tooltip("The range in degrees of the barrel arc in degrees")]
    [MinMaxSlider(-360.0f, 360.0f, true)]
    [SerializeField] Vector2 waveArcRange = new Vector2(-90.0f, 90.0f);
    float ArcMinAngle { get => waveArcRange.x.ToRad(); }
    float ArcMaxAngle { get => waveArcRange.y.ToRad(); }

    [SerializeField] float sineTimeMult = 0.5f;

    [Tooltip("Where will unused waveparts be teleported to until needed?")]
    [SerializeField] Transform unusedPartLocation;


    [SerializeField] float wavePieceZOffset = 0.2f;

    [Tooltip("The starting angle of the wave in degrees")]
    [SerializeField] float waveStartAngle = -90.0f;

    [SerializeField] WaveAnim anim;

    [SerializeField] Rigidbody rb;

    [SerializeField] float barrelRadius = 2.0f;

    [SerializeField] ParticleSystem foamParticles;
    [Tooltip("Foam particles emitted per second - per meter of the wave's length")]
    [SerializeField] float foamParticleCount = 10.0f;

    public float BarrelRadius
    {
        get => barrelRadius; set
        {
            barrelRadius = value;
            dirty = true;
        }
    }

    [SerializeField] float barrelLength = 10.0f;
    public float BarrelLength
    {
        get => barrelLength; set
        {
            barrelLength = value;
            dirty = true;
        }
    }

    [SerializeField] float arc = 1.0f;
    public float BarrelArc { get => arc; set => arc = value; }


    #region Barrel Generated Data

    // Pure functions that calculate new needed information regarding the barrel
    float ArcAngle => Arc * Utils.Tau;

    float CircleCircumference => Radius * Utils.Tau;
    float ArcCircumference => (CircleCircumference / Utils.Tau) * ArcAngle;
    int PiecesPerArc => Mathf.FloorToInt(ArcCircumference / wavePartWidth);
    float AnglePerPart => ArcAngle / PiecesPerArc;

    float PiecesPerCircle => Mathf.FloorToInt(CircleCircumference / wavePartWidth);
    // The angle between each piece to form the full circle
    float FullAnglePerPart => Utils.Tau / PiecesPerCircle;

    Vector3 BarrelCenter => transform.localPosition + transform.up * Radius;
    Vector3 AdjustedCenter => BarrelCenter.WithY(MapHeight(BarrelCenter.y));

    int PiecesPerLength => Mathf.FloorToInt(barrelLength / wavePartLength);

    int PieceX(int index) => Mathf.FloorToInt(index % PiecesPerLength);
    int PieceY(int index) => Mathf.FloorToInt(index / PiecesPerLength);

    #endregion


    // This is set to true if some field that affects whether or not cached bookkeeping values need to be recalculated
    bool dirty = true;

    Vector3 initPos;
    float waveTime = 0.0f;

    void Awake()
    {
        initPos = transform.localPosition;
        mover.MoverController = this;
    }

    void OnEnable() { WaveSettingEvent.RegisterListener(OnWaveSettingsEvent); }

    void OnDisable() { WaveSettingEvent.UnregisterListener(OnWaveSettingsEvent); }

    void Start()
    {
        CalculateCache();
    }

    void CalculateCache()
    {
        wavePartLength = barrelLength;

        //Optimizes the number of pieces we instantiate to only be the amount required
        int piecesPerLength = Mathf.FloorToInt(barrelLength / wavePartLength);
        float circleCircumference = barrelRadius * Utils.Tau;
        wavePartCount = Mathf.CeilToInt(circleCircumference / wavePartWidth) * piecesPerLength;
        wavePartCount += wavePartCount / 2;

        for (int i = waveParts.Count; i < wavePartCount; i++)
        {
            GameObject newPart = Instantiate(
               wavePartPrefab,
               unusedPartLocation.transform.position,
               Quaternion.identity,
               transform);

            waveParts.Add(newPart);
            newPart.name = "WavePart";
            newPart.transform.position = unusedPartLocation.position;
        }

        // Only rescale if we have to, this part takes a long time.
        // We check the last element rather than the first becuase more elements will be added when the barrel radius
        // is increased, this allows us to avoid checking each and every piece.
        if (waveParts.Count > 0 && Mathf.Abs(waveParts[waveParts.Count - 1].transform.localScale.x - wavePartLength) > 0.001f)
        {
            Vector3 wavePartScale = waveParts[0].transform.localScale;
            wavePartScale.x = wavePartLength;
            waveParts.ForEach(n => n.transform.localScale = wavePartScale);
        }

        // TODO: This may be removed, just positions the wave so it's end is always at 0,0.
        // This allows the board's start surf position to be very predictable.
        // transform.position = transform.position.WithX(wavePartLength * 0.5f);

        SetupFoamParticles();

        dirty = false;
    }

    // Update wave movement
    void IMoverController.UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalPosition = initPos + (transform.forward * ForwardPos);
        goalRotation = Quaternion.identity;
    }

    void Update()
    {
        if (!WaveScore.IsWarmup)
            waveTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!WaveScore.IsWarmup)
        {
            if (dirty)
                CalculateCache();

            // This is recalculated each frame in the event we change the radius mid-simulation.
            // This is the length of the wave arc (circle segment)

            if (PiecesPerArc == 0)
            {
                waveParts.ForEach(n => n.transform.position = unusedPartLocation.transform.position);
                return;
            }

            for (int i = 0; i < waveParts.Count; i++)
            {
                // To understand the quadrant system used please refer to 'Assets/Textures/documentation/barrel_quads.png'
                // You might be wondering, why call them quadrants if there are more than four?
                // Well you see, at first there *were* only four quadrants
                switch (Quadrant(i))
                {
                    case BarrelQuadrant.Rise: GenerateRise(i, waveParts[i].transform); break;
                    case BarrelQuadrant.RiseCeil: GenerateRiseCeiling(i, waveParts[i].transform); break;
                    case BarrelQuadrant.FallCeil: GenerateFallCeiling(i, waveParts[i].transform); break;
                    case BarrelQuadrant.Fall: GenerateFall(i, waveParts[i].transform); break;
                    case BarrelQuadrant.RampRight: GenerateRamps(i, waveParts[i].transform, true); break;
                    case BarrelQuadrant.RampLeft: GenerateRamps(i, waveParts[i].transform, false); break;
                    case BarrelQuadrant.Excess: GenerateExcess(i, waveParts[i].transform); break;
                    default: Debug.LogError($"Piece index {i} did not fall into any quadrants"); break;
                }
            }

            if (!foamParticles.gameObject.activeSelf && Arc >= 0.75f)
                foamParticles.gameObject.SetActive(true);
        }

    }

    #region Wave Part Placers

    // The old part-placing method, places pieces in a squashed circular shape
    void GenerateOld(int i, Transform part)
    {
        int x = PieceX(i); // This is mostly useless now
        int y = PieceY(i);

        Vector3 localPos =
                (BarrelCenter + -transform.forward * Radius) // Place forward
                .RotateAround(
                    BarrelCenter,
                    Quaternion.AngleAxis(
                        (waveStartAngle.ToRad() + (((y == PiecesPerArc) ? AnglePerPart : FullAnglePerPart) * y)).ToDeg(),
                        transform.right
                    )
                ) +
                (transform.right * ((wavePartLength * x) + (wavePieceZOffset * y) + (part.transform.localScale.x * 0.5f)));// Move right

        part.localPosition = localPos;

        part.localRotation =
          Quaternion.FromToRotation(
              transform.up,
              (BarrelCenter.WithX(part.localPosition.x) - part.localPosition).normalized
          );

        part.localPosition = part.localPosition.WithY(MapHeight(part.localPosition.y)); // Squish the barrel's height
    }

    void GenerateRise(int i, Transform part) => GenerateOld(i, part);

    void GenerateRiseCeiling(int i, Transform part) => GenerateOld(i, part);

    void GenerateFallCeiling(int i, Transform part) => GenerateOld(i, part);

    void GenerateFall(int i, Transform part) // => GenerateOld(i, part);
    {
        int x = PieceX(i); // This is mostly useless now
        int y = PieceY(i);
        Vector3 localPos =
              (BarrelCenter + -transform.forward * Radius) // Place forward
              .RotateAround(
                  BarrelCenter,
                  Quaternion.AngleAxis(
                      (waveStartAngle.ToRad() + (((y == PiecesPerArc) ? AnglePerPart : FullAnglePerPart) * y)).ToDeg(),
                      transform.right
                  )
              ) +
              (transform.right * ((wavePartLength * x) + (wavePieceZOffset * y) + (part.transform.localScale.x * 0.5f)));// Move right

        localPos.z = Radius; // Clip this to the side so as to form a vertical wall

        part.localPosition = localPos.WithY(MapHeight); // Squish the barrel's height

        part.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.back);
    }

    void GenerateRamps(int i, Transform part, bool right = true)
    {
        // GeneratePool(i, part); return;

        Transform connect = waveParts[i - PiecesPerArc - (right ? 0 : PiecesPerArc / 4)].transform;
        float dir = right ? 1.0f : -1.0f;

        // Attempt to place on barrel ends
        part.position = connect.position + new Vector3(dir * -barrelLength, 0.0f, 0.0f);
        part.rotation = connect.rotation;

        Vector3 rotPos = part.position + (dir * part.right * (barrelLength * 0.5f)); //barrelCenter.WithY(MapHeight) + new Vector3(barrelLength, 0.0f, 0.0f);
        Vector3 rotAxis = part.forward * dir;

        part.RotateAround(rotPos, rotAxis, 5.0f);
    }

    // These are excess pieces not part of the initial barrel
    void GenerateExcess(int i, Transform part) => GeneratePool(i, part);

    #region Excess Piece Usage

    void GeneratePool(int i, Transform part)
    {
        part.transform.position = unusedPartLocation.transform.position;
    }

    #endregion

    #endregion

    // Maps altitude to something lower, allowing for a 'squashed' barrel
    float MapHeight(float h) => h / 2.0f;

    void OnWaveSettingsEvent(WaveSettingEvent e)
    {
        BarrelRadius = e.settings.radius;
        BarrelLength = e.settings.length;
        BarrelArc = e.settings.arc;
    }

    bool UseAnim { get => anim != null; }

    float Radius { get => UseAnim ? anim.barrelRadius.Evaluate(waveTime) * barrelRadius : barrelRadius; }
    float ForwardPos { get => UseAnim ? anim.forwardPosition.Evaluate(waveTime) : transform.position.z; }
    float Arc { get => UseAnim ? anim.barrelArcAngle.Evaluate(waveTime) : arc; }

    void SetupFoamParticles()
    {
        Vector3 foamPos = transform.position +
            (transform.right * (barrelLength * 0.5f)) +
            (transform.forward * barrelRadius * 0.8f);

        foamParticles.transform.position = foamPos;

        EmissionModule em = foamParticles.emission;
        em.rateOverTime = new MinMaxCurve(foamParticleCount * barrelLength);

        ShapeModule sh = foamParticles.shape;
        sh.scale = new Vector3(barrelLength, 1.0f, 1.0f);
    }

    BarrelQuadrant Quadrant(int i)
    {
        int y = PieceY(i);
        if (y > PiecesPerArc)
            return BarrelQuadrant.Excess; // Evaluated to none of the above
        if (y < 0) return BarrelQuadrant.Err; // A negative index?
        if (y <= PiecesPerCircle / 4) return BarrelQuadrant.Rise;
        if (y <= PiecesPerCircle / 2) return BarrelQuadrant.RiseCeil;
        if (y <= 3 * (PiecesPerCircle / 4)) return BarrelQuadrant.FallCeil;
        if (y <= PiecesPerCircle) return BarrelQuadrant.Fall;
        if (y <= PiecesPerCircle + (PiecesPerCircle / 4)) return BarrelQuadrant.RampRight;
        if (y <= PiecesPerCircle + (PiecesPerCircle / 2)) return BarrelQuadrant.RampLeft;
        return BarrelQuadrant.Err;
    }
}

public enum BarrelQuadrant
{
    Rise, // This quarter of the barrel is the part the player should be surfing on
    RiseCeil, // This is the barrel ceiling, on the rising end
    FallCeil, // This is the barrel ceiling, on the falling end
    Fall, // This is the last part of the barrel, and should be falling vertically
    RampRight, // This piece is a right-side ramp piece
    RampLeft, // This piece is a left-side ramp piece

    Err, // This value should never show up in the rest of the program
    Excess, // This piece is an excess piece
}
