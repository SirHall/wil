using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Excessives;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class CameraPositions : MonoBehaviour
{
    [SerializeField] Transform camTransform;

    [SerializeField] Transform targetTransformsParent;

    IEnumerable<Transform> TransformPositions { get => targetTransformsParent.GetComponentsInChildren<Transform>().Where(n => n != transform); }

    [SerializeField] InputAction camNext;
    [SerializeField] InputAction camPrev;

    int currentI = 0;

    void OnEnable()
    {
        camNext.Enable();
        camPrev.Enable();
    }

    void OnDisable()
    {
        camNext.Disable();
        camPrev.Disable();
    }

    void Start()
    {
        SetCamera();
        camNext.performed += e => Increase();
        camPrev.performed += e => Decrease();
    }

    public int CurrentIndex
    {
        get => currentI;
        set
        {
            currentI = MathE.ClampWrap(value, 0, TransformPositions.Count());
            SetCamera(); // Yes I'm putting behaviour in a property :)
        }
    }

    public Transform CurrentTransform { get => TransformPositions.ElementAt(CurrentIndex); }

    [ButtonGroup()] public void Increase() => CurrentIndex++;
    [ButtonGroup()] public void Decrease() => CurrentIndex--;

    [ButtonGroup()]
    void SetCamera()
    {
        camTransform.position = CurrentTransform.position;
        camTransform.rotation = CurrentTransform.rotation;
    }
}
