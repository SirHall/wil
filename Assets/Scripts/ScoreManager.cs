using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    int warningAmt;
    float warningTime;

    void OnEnable()
    {
        ScoreControlEvent.RegisterListener(OnWarningStateEvent);
    }

    void OnDisable()
    {
        ScoreControlEvent.UnregisterListener(OnWarningStateEvent);
    }

    // A controller has announced new data
    void OnWarningStateEvent(ScoreControlEvent e)
    {
        warningAmt = e.warningAmt;
        warningTime = e.warningTime;
    }

    void Update()
    {
        // Used for testing
        print("Warning Amount: " + warningAmt);
        print("Warning Time: " + warningTime);
    }
}
