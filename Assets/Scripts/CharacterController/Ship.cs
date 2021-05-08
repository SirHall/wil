using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class Ship : MonoBehaviour, IMoverController
{

    [SerializeField] PhysicsMover mover;

    [SerializeField] float vel = 1.0f;

    void Awake() { mover.MoverController = this; }

    void Start() { }

    void Update() { }

    void IMoverController.UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalPosition = transform.position + transform.forward * deltaTime * vel;
        goalRotation = transform.rotation;
    }
}
