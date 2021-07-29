using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// This component's single purpose is to display a comment in the editor
public class Comment : MonoBehaviour
{
    [HideLabel]
    [TextArea]
    [SerializeField] string comment = "";
}
