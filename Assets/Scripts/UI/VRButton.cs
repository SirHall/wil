using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{

    #region Button Info

    [Tooltip("The distance the button must be moved from its starting position by the hand to be considered 'pressed'")]
    [TabGroup("Settings")]
    [SerializeField] float depressDist = 0.01f;

    [Tooltip("If enabled, the button will move back to its original position after being depressed")]
    [TabGroup("Settings")]
    [SerializeField] bool momentarySwitch = true;

    [TabGroup("Settings")]
    [SerializeField] VRButtons button = VRButtons.None;

    [Tooltip("How quickly the button returns to it's original position after being fully depressed")]
    [TabGroup("Settings")]
    [SerializeField] float buttonRiseLerpSpeed = 1.0f;

    [Tooltip("When the button has returned within this range of its original position, it has been 'unpressed'")]
    [TabGroup("Settings")]
    [SerializeField] float unpressDist = 0.1f;

    [Tooltip("When pressed by a non-physical source (user clicks on this button using their mouse), how far does the butto depress")]
    [TabGroup("Settings")]
    [SerializeField] float clickDist = 1.0f;

    [Tooltip("What is displayed on the button as text")]
    [OnValueChanged("@this.text.text = this.label")]
    [TabGroup("Settings")]
    [SerializeField] string label = "Press";

    [Tooltip("The color of the button in play")]
    [TabGroup("Settings")]
    [SerializeField] Color buttonColor = Color.black;

    [Tooltip("If whether the button should rotate itself to be pointed towards it's local origin")]
    [TabGroup("Settings")]
    [SerializeField] bool orientTowardsOrigin = true;

    #endregion

    [TabGroup("References")]
    [SerializeField] MeshRenderer meshRend;

    [TabGroup("References")]
    [SerializeField] TextMeshPro text;

    [TabGroup("References")]
    [SerializeField] UnityEvent OnPressed;

    [TabGroup("References")]
    [SerializeField] UnityEvent OnLift;

    #region Bookkeeping

    Vector3 initPos;

    bool pressed = false; // This ensures that a button only fires for one frame until it reaches it's original position again

    #endregion

    void Start()
    {
        initPos = transform.localPosition;

        text.text = label;
        meshRend.material.color = buttonColor;
    }

    void Update()
    {
        if (orientTowardsOrigin && transform.parent != null)
            transform.LookAt(transform.parent.position, Vector3.up);

        // We move the button back to it's original position on the frame after it has been fully depressed
        if (pressed && momentarySwitch)
            transform.localPosition = Vector3.Lerp(transform.localPosition, initPos, Time.deltaTime * buttonRiseLerpSpeed);

        if (pressed && momentarySwitch && Vector3.Distance(initPos, transform.localPosition) <= unpressDist)
        {
            pressed = false; // This button has now returned to its original position, and is now 'unpressed'
            transform.localPosition = initPos;
            OnLift.Invoke();
        }

        if (!pressed && Vector3.Distance(initPos, transform.localPosition) >= depressDist)
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

        OnPressed.Invoke();
    }

    public void Press()
    {
        ButtonPressed();
        transform.localPosition = initPos - (transform.forward * Mathf.Max(depressDist, clickDist));
    }
}