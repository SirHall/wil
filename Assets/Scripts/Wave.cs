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
    [SerializeField] int wavePartCount = 100;

    [SerializeField] float wavePartLength = 1.0f;
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
        waveTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (dirty)
            CalculateCache();

        float arcAngle = Arc * Utils.Tau;

        // This is recalculated each frame in the event we change the radius mid-simulation.
        // This is the length of the wave arc (circle segment)
        float circleCircumference = Radius * Utils.Tau;
        float arcCircumference = (circleCircumference / Utils.Tau) * arcAngle;
        int piecesPerArc = Mathf.FloorToInt(arcCircumference / wavePartWidth);
        float anglePerPart = arcAngle / piecesPerArc;

        float piecesPerCircle = Mathf.FloorToInt(circleCircumference / wavePartWidth);
        // The angle between each piece to form the full circle
        float fullAnglePerPart = Utils.Tau / piecesPerCircle;

        Vector3 barrelCenter = transform.localPosition + transform.up * Radius;
        Vector3 adjustedCenter = barrelCenter.WithY(MapHeight(barrelCenter.y));

        int piecesPerLength = Mathf.FloorToInt(barrelLength / wavePartLength);

        if (piecesPerArc == 0)
        {
            waveParts.ForEach(n => n.transform.position = unusedPartLocation.transform.position);
            return;
        }

        for (int i = 0; i < waveParts.Count; i++)
        {
            GameObject n = waveParts[i];
            int x = Mathf.FloorToInt(i % piecesPerLength);
            int y = Mathf.FloorToInt(i / piecesPerLength);

            if (y > piecesPerArc)
            {
                n.transform.position = unusedPartLocation.transform.position;
                continue;
            }


            Vector3 localPos =
            (
                (barrelCenter + -transform.forward * Radius) // Place forward
                .RotateAround(
                    barrelCenter,
                    Quaternion.AngleAxis(
                        (waveStartAngle.ToRad() + (((y == piecesPerArc) ? anglePerPart : fullAnglePerPart) * y)).ToDeg(),
                        transform.right
                    )
                ) +
                (transform.right * ((wavePartLength * x) + (wavePieceZOffset * y) + (n.transform.localScale.x * 0.5f))) // Move right
            );

            n.transform.localPosition = localPos;

            n.transform.localRotation =
              Quaternion.FromToRotation(
                  transform.up,
                  (barrelCenter.WithX(n.transform.localPosition.x) - n.transform.localPosition).normalized
              );

            n.transform.localPosition = n.transform.localPosition.WithY(MapHeight(n.transform.localPosition.y)); // Squish the barrel's height
        }

        if (!foamParticles.gameObject.activeSelf && Arc >= 0.75f)
            foamParticles.gameObject.SetActive(true);
    }

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
}
