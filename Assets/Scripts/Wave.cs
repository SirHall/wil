using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives;
using Excessives.LinqE;
using Excessives.Unity;
using Sirenix.OdinInspector;
using KinematicCharacterController;

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

    void Awake() { mover.MoverController = this; }

    void Start()
    {
        CalculateCache();
    }

    void CalculateCache()
    {
        initPos = transform.localPosition;
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
        }

        waveParts.ForEach(n => n.name = "WavePart");

        waveParts.ForEach(n => n.transform.position = unusedPartLocation.position);

        waveParts.ForEach(n =>
        {
            Vector3 wavePartScale = n.transform.localScale;
            wavePartScale.x = wavePartLength;
            n.transform.localScale = wavePartScale;
        });

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
        if (dirty)
            CalculateCache();
    }

    void FixedUpdate()
    {
        // rb.MovePosition(originalPos + (transform.forward * anim.forwardPosition.Evaluate(animTime)));
        // transform.localPosition = originalPos + (transform.forward * anim.forwardPosition.Evaluate(animTime));

        float arcAngle = Arc * Utils.Tau;

        //Need some way to figure out where to place each panel to create a barrel wave
        // float maxAngle = Utils.nSin(waveMaxRad + Time.time * sineTimeMult);
        // float arcAngle = ArcMinAngle + Mathf.Abs(ArcMaxAngle - ArcMinAngle) * Utils.nSin(Time.time * sineTimeMult);
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

            if (y > piecesPerArc)//|| y * fullAnglePerPart > arcAngle)
            {
                n.transform.position = unusedPartLocation.transform.position;
                continue;
            }

            // if (y < piecesPerArc - 1)
            //     continue;

            // if (y < piecesPerArc - 1) continue;

            // rb.position =
            n.transform.localPosition =
                (barrelCenter + -transform.forward * Radius) // Place forward
                .RotateAround(
                    barrelCenter,
                    Quaternion.AngleAxis(
                        (waveStartAngle.ToRad() + (((y == piecesPerArc) ? anglePerPart : fullAnglePerPart) * y)).ToDeg(),
                        transform.right
                    )
                ) +
                (transform.right * ((wavePartLength * x) + (wavePieceZOffset * y))); // Move right

            // rb.rotation =
            n.transform.localRotation =
            Quaternion.FromToRotation(
                transform.up,
                (barrelCenter.WithX(n.transform.localPosition.x) - n.transform.localPosition).normalized
            );
            // A little over rotation so the board
            // n.transform.RotateAround(n.transform.position, Vector3.forward, );
        }
    }

    bool UseAnim { get => anim != null; }

    float Radius { get => UseAnim ? anim.barrelRadius.Evaluate(waveTime) : barrelRadius; }
    float ForwardPos { get => UseAnim ? anim.forwardPosition.Evaluate(waveTime) : transform.position.z; }
    float Arc { get => UseAnim ? anim.barrelArcAngle.Evaluate(waveTime) : arc; }
}
