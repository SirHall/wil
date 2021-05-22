using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] InputAction resetInput;

    void Start()
    {
        BarrelSettings.EmitBarrelSettings();
    }

    void OnEnable()
    {
        resetInput.Enable();
    }

    void Update()
    {
        if (resetInput.ReadValue<float>() > 0.5f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reset game
    }

    void OnDisable()
    {
        resetInput.Disable();
    }
}
