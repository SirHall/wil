using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.LinqE;

public class RecursivePreventSelfCollisions : MonoBehaviour
{
    //Go through each child and set its collider to ignore all other colliders in our gameObject
    void Start() => GetComponentsInChildren<Collider>().Combination(Physics.IgnoreCollision);
}
