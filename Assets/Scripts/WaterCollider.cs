using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    [SerializeField] Collider sourceCollider;
    [SerializeField] Rigidbody rb;

    [Tooltip("Buoyancy force for this surfboard")]
    [SerializeField] float buoyancyForce = 10.0f;

    void Start()
    {
        sourceCollider = sourceCollider ?? GetComponent<Collider>();
        // We need to disable the standard collisions on this collider
        sourceCollider.isTrigger = true;
    }

    void OnTriggerStay(Collider otherCollider)
    {
        // Direction of intersection
        Vector3 intDir = transform.position - otherCollider.ClosestPoint(transform.position);
        if (intDir.magnitude < 0.0001f)
        {
            // We are inside the other collider, apply force to get us out

        }

        // // Intersection depth
        float depth = intDir.magnitude;

        // rb.AddForce((intDir.normalized / depth) * buoyancyForce);
        // Debug.DrawRay(transform.position, intDir.normalized / depth, Color.red, Time.fixedDeltaTime, false);
    }
}
