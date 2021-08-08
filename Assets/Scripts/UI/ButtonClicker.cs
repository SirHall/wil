using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonClicker : MonoBehaviour
{
    [SerializeField] InputAction buttonClick;

    [SerializeField] float maxDist = 2.0f;

    void OnEnable()
    {
        buttonClick.Enable();
        buttonClick.performed += Click;
    }

    void OnDisable()
    {
        buttonClick.Disable();
        buttonClick.performed -= Click;
    }

    void Click(InputAction.CallbackContext e)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDist))
        {
            VRButton button =
                hit.collider?.GetComponent<VRButton>() ??
                hit.collider?.transform?.parent?.GetComponent<VRButton>() ??
                hit.collider?.transform?.root?.GetComponent<VRButton>();
            if (button)
                button.Press();
        }
    }
}
