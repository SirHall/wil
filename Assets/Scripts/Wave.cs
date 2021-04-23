using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives;
using Excessives.LinqE;
using Excessives.Unity;
using Sirenix.OdinInspector;

//         ______________________________________
//        |                 NOTE                 |
//        |THE BELOW USES RADIANS FOR ALL ANGLES!|
//        |    Except where otherwise stated     |
//         ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾

public class Wave : MonoBehaviour
{

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

    [SerializeField] float barrelRadius = 2.0f;

    [Tooltip("Where will unused waveparts be teleported to until needed?")]
    [SerializeField] Transform unusedPartLocation;

    [SerializeField] float barrelLength = 10.0f;

    [SerializeField] float wavePieceZOffset = 0.2f;

    [Tooltip("The starting angle of the wave in degrees")]
    [SerializeField] float waveStartAngle = -90.0f;

    void Start()
    {
        for (int i = 0; i < wavePartCount; i++)
            waveParts.Add(Instantiate(
                wavePartPrefab,
                unusedPartLocation.transform.position,
                Quaternion.identity,
                transform)
            );
        waveParts.ForEach(n => n.name = "WavePart");

        Color[] colors = new Color[] { Color.blue, Color.red, Color.green, Color.yellow };

        waveParts.For((n, i) =>
            n.GetComponent<MeshRenderer>().material.color = colors[i % colors.Length]);
    }

    void FixedUpdate()
    {
        //Need some way to figure out where to place each panel to create a barrel wave
        // float maxAngle = Utils.nSin(waveMaxRad + Time.time * sineTimeMult);
        float arcAngle = ArcMinAngle + Mathf.Abs(ArcMaxAngle - ArcMinAngle) * Utils.nSin(Time.time * sineTimeMult);
        // This is recalculated each frame in the event we change the radius mid-simulation.
        // This is the length of the wave arc (circle segment)
        float circleCircumference = barrelRadius * Utils.Tau;
        float arcCircumference = (circleCircumference / Utils.Tau) * arcAngle;
        int piecesPerArc = Mathf.FloorToInt(arcCircumference / wavePartWidth);
        float anglePerPart = arcAngle / piecesPerArc;

        float piecesPerCircle = Mathf.FloorToInt(circleCircumference / wavePartWidth);
        // The angle between each piece to form the full circle
        float fullAnglePerPart = Utils.Tau / piecesPerCircle;

        Vector3 barrelCenter = transform.position + Vector3.up * barrelRadius;

        int piecesPerLength = Mathf.FloorToInt(barrelLength / wavePartLength);

        if (piecesPerArc == 0)
        {
            waveParts.ForEach(n => n.transform.position = unusedPartLocation.transform.position);
            return;
        }

        waveParts.For((n, i) =>
            {
                int x = Mathf.FloorToInt(i % piecesPerLength);
                int y = Mathf.FloorToInt(i / piecesPerLength);

                if (y > piecesPerArc)//|| y * fullAnglePerPart > arcAngle)
                {
                    n.transform.position = unusedPartLocation.transform.position;
                    return;
                }

                n.transform.position =
                (barrelCenter + Vector3.right * barrelRadius) // Place right
                .RotateAround(
                    barrelCenter,
                    Quaternion.AngleAxis(
                        (waveStartAngle.ToRad() + (fullAnglePerPart * y)).ToDeg(),
                        Vector3.forward
                    )
                ) +
                (Vector3.forward * ((wavePartLength * x) + (wavePieceZOffset * y))); // Move forward

                n.transform.rotation =
                    Quaternion.FromToRotation(
                        Vector3.up,
                        (barrelCenter.WithZ(n.transform.position.z) - n.transform.position).normalized
                    );
            });
    }
}
