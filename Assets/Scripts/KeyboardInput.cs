using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class KeyboardInput : MonoBehaviour
{
    [SerializeField] [TitleGroup("Movement")] InputAction cardinalInput;
    Vector2 Cardinal { get => cardinalInput.ReadValue<Vector2>(); }


    [SerializeField] [MinMaxSlider(0.0f, 180.0f, true)] [TitleGroup("Head")] Vector2 elevationRange = new Vector2(0.0f, 180.0f);

    [SerializeField] [TitleGroup("Head")] InputAction mouseDelta = new InputAction();

    [Tooltip("This should be the transform of the gameobject to which the camer is attached")]
    [SerializeField] [TitleGroup("Head")] Transform viewTransform;

    [SerializeField] [TitleGroup("Head")] float sensitivity = 0.1f;

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

    void Start() { prev = Cardinal; }

    void Update()
    {
        if ((Cardinal - prev).sqrMagnitude > 0.01f)
            using (var e = BoardControlEvent.Get())
                e.input.dir = Cardinal;

        prev = Cardinal;

        // currentCamRotation.x +=;
        // currentCamRotation.y += sensitivity * -mouseDelta.ReadValue<Vector2>().y; // Mathf.Clamp(currentCamRotation.y + mouseDelta.ReadValue<Vector2>().x, elevationRange.x, elevationRange.y);

        if (MouseLock.IsMouseLocked)
        {
            viewTransform.Rotate(Vector3.up, sensitivity * mouseDelta.ReadValue<Vector2>().x, Space.World);
            viewTransform.Rotate(Vector3.left, sensitivity * mouseDelta.ReadValue<Vector2>().y, Space.Self);
        }
    }
}
