using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ResetPosition : MonoBehaviour
{
    [SerializeField] InputAction resetPositionInput;

    public GameObject surfboard;

    public Vector3 resetPosition;
    public Vector3 resetRotation;
    void OnEnable() {
        resetPositionInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (resetPositionInput.ReadValue<float>() > 0.5f) 
        {
            
        }
    }

    void OnDisable() {
        resetPositionInput.Disable();
    }
}
