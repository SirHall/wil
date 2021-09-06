using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadCollision : MonoBehaviour
{
    private GameObject activeCamera;
    private List<GameObject> collidedObjects = new List<GameObject>();
    //private float averageDistanceValue = 10;
    private float shortestDistance = 10;

    private float lerp = 0f;

    // Start is called before the first frame update
    void Start()
    {
        activeCamera = Camera.main.gameObject;
        GenerateCollision(activeCamera);
    }

    void Update()
    {
        if (collidedObjects.Count > 0)
        {
            List<float> distances = new List<float>();
            foreach (GameObject collidedObject in collidedObjects)
            {
                Vector3 closestPoint = collidedObject.GetComponentInChildren<MeshRenderer>().bounds.ClosestPoint(activeCamera.transform.position);
                float distance = Vector3.Distance(activeCamera.transform.position, closestPoint);
                distances.Add(distance);
                //averageDistanceValue += distance;
            }

            //averageDistanceValue = averageDistanceValue / collidedObjects.Count;
            shortestDistance = distances.Min();
        }
        else
        {
            //averageDistanceValue = 10;
            shortestDistance = 10;
        }
            

        //HeadToWaterDistance(averageDistanceValue);
        //print("Average Distance: " + averageDistanceValue);

        HeadToWaterDistance(shortestDistance);
        print("Shortest Distance: " + shortestDistance);
    }

    void GenerateCollision(GameObject collisionObject)
    {
        SphereCollider headCollider = collisionObject.AddComponent<SphereCollider>();
        headCollider.isTrigger = true;
        headCollider.center = Vector3.zero;
        headCollider.radius = 1f;
    }

    void HeadToWaterDistance(float distance)
    {
        //IN DEVELOPMENT
        float alphaValue;

        float furthestDistance = 1.5f;
        if (distance <= furthestDistance)
            alphaValue = furthestDistance - distance; // Example: 1 -> 0 becomes 0 -> 1
        else
            alphaValue = 0;

        print("Alpha value: " + alphaValue);

        float clampedAlpha = Mathf.Clamp(Mathf.Abs(alphaValue), 0, 0.8f) / 0.8f;

        print("Clamped: " + clampedAlpha);

        using (var e = WaterScreenEvent.Get())
            e.alphaValue = clampedAlpha;

        //Toggle visual wet effect
        //Value will increase as distance gets closer to 0
        //Toogle blur effect
        //Toggle blue haze effect

        // All effects together will alert the player their head is close to the barrel

        if (distance <= 0f) // Fallen
        {
            //using (var e = GameLost.Get())
            //    e.cause = "Head collided with barrel";

            print("Died");

            return;
        }
        else if (distance <= 0.3f)
        {
            print("Getting Close");
            return;
        }
            
        else if (distance <= 0.6)
        {
            print("Careful with your head");
            // Woah sound effect quiter
            return;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<InteractionType>() != null)
        {
            if (other.gameObject.GetComponent<InteractionType>().interactable == Interactables.Water)
            {
                collidedObjects.Add(other.gameObject);

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<InteractionType>() != null)
        {
            if (other.gameObject.GetComponent<InteractionType>().interactable == Interactables.Water)
            {
                if (collidedObjects.Contains(other.gameObject))
                    collidedObjects.Remove(other.gameObject);
            }
        }
    }
}
