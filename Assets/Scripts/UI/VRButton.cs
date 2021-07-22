using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButton : MonoBehaviour
{
    [Tooltip("The distance the button must be moved from its starting position by the hand to be considered 'pressed'")]
    [SerializeField] float depressDist = 0.5f;

    [Tooltip("If enabled, the button will move back to its original position after being depressed")]
    [SerializeField] bool momentarySwitch = true;

    [SerializeField] VRButtons button = VRButtons.None;

    [Tooltip("How quickly the button returns to it's original position after being fully depressed")]
    [SerializeField] float buttonRiseLerpSpeed = 1.0f;

    #region Bookkeeping

    Vector3 initPos;

    bool pressed = false; // This ensures that a button only fires for one frame until it reaches it's original position again

    #endregion

    void Start()
    {
        initPos = transform.position;
    }

    void Update()
    {
        // We move the button back to it's original position on the frame after it has been fully depressed
        if (pressed && momentarySwitch)
            transform.position = Vector3.Lerp(transform.position, initPos, Time.deltaTime * buttonRiseLerpSpeed);

        if (pressed && momentarySwitch && Vector3.Distance(initPos, transform.position) <= 0.01f)
            pressed = false; // This button has now returned to its original position, and is now 'unpressed'

        if (!pressed && Vector3.Distance(initPos, transform.position) >= depressDist)
            ButtonPressed();
    }

    void ButtonPressed()
    {
        pressed = true;

        if (button == VRButtons.None)
        {
            print("Ensure that all VR Button's have their 'button' field set to a button type other than 'None'");
            return; // If this button does nothing, do not call the VRButtonEvent
        }

        using (var e = VRButtonEvent.Get())
            e.button = button;
    }
}
