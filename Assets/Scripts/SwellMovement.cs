using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwellMovement : MonoBehaviour
{
    public float speed = 1.0F;

    public Vector3 startPos;
    public Vector3 endPos;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (WaveScore.IsWarmup) return;

        if (!isMoving && !WaveScore.IsWarmup)
        {
            startTime = Time.time;
            isMoving = true;

            // Calculate the journey length.
            journeyLength = Vector3.Distance(startPos, endPos);
        }

        if (isMoving)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
        }

        
    }

}
