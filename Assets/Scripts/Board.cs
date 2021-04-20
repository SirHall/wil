using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.Unity;
using UnityEngine.InputSystem;

public class Board : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public Rigidbody RB { get => rb; }

    [Tooltip("Thrust add to board when 'moving forward'")]
    [SerializeField]
    float thrust = 1.0f; // Thrust generated by the board (How is thrust generated?)

    [Tooltip("When turning board, apply this rotational force")]
    [SerializeField]
    float rotThrust = 1.0f;

    [SerializeField] InputAction cardinalInput;

    Vector2 Cardinal { get => cardinalInput.ReadValue<Vector2>(); }

    // [SerializeField] float downForce = 5.0f;

    [SerializeField] float waveForce = 10.0f;

    void OnEnable() { cardinalInput.Enable(); }

    void Start()
    {

    }

    void OnCollisionStay(Collision col)
    {
        if (col.collider.name == "WavePart")
            rb.AddForce(Vector3.Lerp(col.transform.right, col.transform.forward, 0.75f) * waveForce);
    }

    void FixedUpdate()
    {
        // Move board with keys at the start, use mouse to move camera

        // We are applying a force rather than direct velocity as it will also help when simulating
        // the difficulty of the board from going 'uphill' in the barrel.

        rb.AddForce(transform.forward * thrust * Mathf.Clamp01(Cardinal.y), ForceMode.Force);

        // With 'A', 'D' add rotational force
        // Would be nice to have an 'AddRelativeTorqueAtPoint' to add this torque from the board fins
        rb.AddRelativeTorque(Vector3.up * rotThrust * Cardinal.x, ForceMode.Force);

        // Keep board glued to the 'ground' using a downward thrust
        // rb.AddForce(-transform.up * downForce);


        // Apply a force to the board attempting to re-right it if it isn't
        // colliding with anything and is not the correct way up
        // rb.AddForceAtPosition(
        //     ((transform.position + Vector3.up) - (transform.position + transform.up)) * 0.5f,
        //     transform.position + transform.up,
        //     ForceMode.Force
        // );
    }

    void OnDisable() { cardinalInput.Disable(); }
}
