using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInput : MonoBehaviour
{
    [SerializeField] InputAction cardinalInput;
    Vector2 Cardinal { get => cardinalInput.ReadValue<Vector2>(); }

    void OnEnable() => cardinalInput.Enable();

    void OnDisable() => cardinalInput.Disable();

    Vector2 prev;

    void Start() { prev = Cardinal; }

    void Update()
    {
        if ((Cardinal - prev).sqrMagnitude > 0.01f)
            using (var e = BoardControlEvent.Get())
                e.input.dir = Cardinal;

        prev = Cardinal;
    }
}
