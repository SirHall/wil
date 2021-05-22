using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    ScoreInput input = new ScoreInput();

    void OnEnable() {
        ScoreControlEvent.RegisterListener(OnWarningStateEvent);
    }

    void OnDisable() {
        ScoreControlEvent.UnregisterListener(OnWarningStateEvent);
    }

    // A controller has announced new data
    void OnWarningStateEvent(ScoreControlEvent e) {
        input = e.input;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Used for testing
        print("Warning Amount: " + input.warningAmt);
        print("Warning Time: " + input.warningTime);
    }
}
