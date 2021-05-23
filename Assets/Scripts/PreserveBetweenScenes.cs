using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreserveBetweenScenes : MonoBehaviour
{
    void Awake() => DontDestroyOnLoad(gameObject);
}
