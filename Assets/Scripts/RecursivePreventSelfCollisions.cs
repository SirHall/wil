using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursivePreventSelfCollisions : MonoBehaviour
{
    public bool applyToBaseObject = false;

    //Go through each child and set its collider to ignore all other colliders in our gameObject
    void Awake()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        if (applyToBaseObject)
        {
            Collider ownCollider = GetComponent<Collider>();
            for (int k = 0; k < colliders.Length; k++)
                Physics.IgnoreCollision(ownCollider, colliders[k]);
        }

        for (int i = 0; i < colliders.Length; i++)
            for (int j = i; j < colliders.Length; j++)
                Physics.IgnoreCollision(colliders[i], colliders[j]);
    }
}
