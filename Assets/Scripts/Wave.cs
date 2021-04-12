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

    void Start()
    {
        for (int i = 0; i < wavePartCount; i++)
            waveParts.Add(Instantiate(wavePartPrefab));
    }

    void Update()
    {
        //Need some way to figure out where to place each panel to create a barrel wave
        // float maxAngle = Utils.nSin(waveMaxRad + Time.time * sineTimeMult);
        float arcAngle = (ArcMaxAngle - ArcMinAngle) * Utils.nSin(Time.time * sineTimeMult);
        // This is recalculated each frame in the event we change the radius mid-simulation.
        // This is the length of the wave arc (circle segment)
        float barrelCircumference = (barrelRadius * Utils.Tau) / Utils.Tau * arcAngle;
        int piecesPerArc = Mathf.CeilToInt(barrelCircumference / wavePartWidth);
        float anglePerPart = arcAngle / piecesPerArc;

        Vector3 barrelCenter = transform.position + Vector3.up * barrelRadius;

        if (piecesPerArc == 0)
        {
            waveParts.ForEach(n => n.transform.position = unusedPartLocation.transform.position);
            return;
        }

        waveParts.For((n, i) =>
            {
                int row = Mathf.FloorToInt(i / piecesPerArc);
                if (row * wavePartLength > barrelLength)
                {
                    n.transform.position = unusedPartLocation.transform.position;
                    return;
                }

                n.transform.position =
                (barrelCenter + Vector3.right * barrelRadius)
                .RotateAround(
                    barrelCenter,
                    Quaternion.AngleAxis(
                        (ArcMinAngle + anglePerPart * (i % piecesPerArc)).ToDeg(),
                        Vector3.forward
                    )
                ) +
                (Vector3.forward * ((row * wavePartLength) + (wavePieceZOffset * (i % piecesPerArc))));

                n.transform.rotation =
                    Quaternion.FromToRotation(
                        Vector3.up,
                        (barrelCenter.WithZ(n.transform.position.z) - n.transform.position).normalized
                    );
            });
    }
}
