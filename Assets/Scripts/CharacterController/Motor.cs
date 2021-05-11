using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.Unity;

public class Motor : MonoBehaviour
{
    [SerializeField] float moveVel = 5.0f;

    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;

    void Start()
    {

    }

    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        UnityExcessives.IfKeys(
            KeyCode.W, KeyCode.S,
            KeyDetectMode.Held, KeyDetectMode.Held,
            () => moveDir.z = 1.0f,
            () => moveDir.z = 0.0f
        );

        UnityExcessives.IfKeys(
            KeyCode.D, KeyCode.A,
            KeyDetectMode.Held, KeyDetectMode.Held,
            () => moveDir.x = 1.0f,
            () => moveDir.x = 0.0f
        );
    }
}
