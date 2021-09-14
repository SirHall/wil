using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Input Definition", menuName = "Input/Input System Definition", order = 1)]
public class InputSystemDefinition : ScriptableObject
{
    [Tooltip("The avaiability object, this destroys itself in Start() if the input mode is not available")]
    public GameObject available;

    [Tooltip("The highest-priority available input system will be selected")]
    public float priority = 0.0f;

    [Tooltip("The prefab placed on the surfboard if this input system is selected")]
    public GameObject character;
}
