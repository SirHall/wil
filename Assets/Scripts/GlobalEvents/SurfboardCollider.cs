using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfboardCollider : MonoBehaviour
{
    void Start()
    {
        GetComponent<Collider>().enabled = false;
    }
}
