using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Input Definition List", menuName = "Input/Input Definition List", order = 1)]
public class InputDefinitionList : ScriptableObject
{
    [Tooltip("Add all input system definitions to be used by the system to this list")]
    public List<InputSystemDefinition> definitions = new List<InputSystemDefinition>();
}
