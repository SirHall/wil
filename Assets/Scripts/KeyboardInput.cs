using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Excessives.Unity;

public class KeyboardInput : MonoBehaviour
{
    [SerializeField] [TitleGroup("Movement")] InputAction cardinalInput;
    Vector2 Cardinal { get => cardinalInput.ReadValue<Vector2>(); }

    [SerializeField] [MinMaxSlider(0.0f, 180.0f, true)] [TitleGroup("Head")] Vector2 elevationRange = new Vector2(0.0f, 180.0f);

    [SerializeField] [TitleGroup("Head")] InputAction mouseDelta = new InputAction();

    [Tooltip("This should be the transform of the gameobject to which the camera is attached")]
    [SerializeField] [TitleGroup("Head")] Transform viewTransform;

    // TODO: Add code to allow this value to be changed after instantiation and correct the camera appropriately
    [Tooltip("Whether the camera is able to rotate freely, or if it is used in steer mode")]
    [SerializeField] [TitleGroup("Head")] KeyboardHeadMode headMode = KeyboardHeadMode.View;
    [Tooltip("Sensitivity of the mouse when using the camera in view mode")]
    [SerializeField] [TitleGroup("Head")] float sensitivity = 0.1f;
    [Tooltip("Sensitivity of the camera in steer mode")]
    [SerializeField] [TitleGroup("Head")] float mouseSteerSensitivity = 1.0f;

    void OnEnable()
    {
        mouseDelta.Enable();
        cardinalInput.Enable();
    }

    void OnDisable()
    {
        mouseDelta.Disable();
        cardinalInput.Disable();
    }


    #region Bookkeeeping

    Vector2 currentCamRotation = new Vector2(0.0f, 0.0f);

    #endregion


    Vector2 prev;
    Vector3 viewInitPos;

    void Start()
    {
        prev = Cardinal;
        viewInitPos = viewTransform.localPosition;
    }

    void Update()
    {
        // if ((Cardinal - prev).sqrMagnitude > 0.01f)
        //     using (var e = BoardControlEvent.Get())
        //         e.input.dir = Cardinal;


        // currentCamRotation.x +=;
        // currentCamRotation.y += sensitivity * -mouseDelta.ReadValue<Vector2>().y; // Mathf.Clamp(currentCamRotation.y + mouseDelta.ReadValue<Vector2>().x, elevationRange.x, elevationRange.y);

        Vector2 delta = mouseDelta.ReadValue<Vector2>();

        if (MouseLock.IsMouseLocked)
        {
            if (headMode == KeyboardHeadMode.View)
            {
                viewTransform.Rotate(Vector3.up, sensitivity * delta.x, Space.World);
                viewTransform.Rotate(Vector3.left, sensitivity * delta.y, Space.Self);
            }
            // I know there aren't any other options, this is here for readability
            else if (headMode == KeyboardHeadMode.Steer)
            {
                viewTransform.localPosition += mouseSteerSensitivity * delta.xyy().WithY(0.0f);
            }
        }

        using (var e = BoardControlEvent.Get())
            e.input.dir = (viewTransform.localPosition - viewInitPos).xy();

        // prev = Cardinal;
    }
}

public enum KeyboardHeadMode
{
    View, // The head is viewing the world around it (like a FPS)
    Steer // The head is being used to steer the board
}