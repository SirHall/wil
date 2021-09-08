using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadCollision : MonoBehaviour
{
    [Tooltip("List of all the collided gameobjects")]
    private List<GameObject> collidedObjects = new List<GameObject>();

    [Tooltip("The size of the head collider to detect surrounding wave objects")]
    public float headColliderRadius = 1f;

    [Tooltip("The shortest distance found to another wave object")]
    private float shortestDistance;

    [Tooltip("The remaining and primary active camera in the scene hierarchy")]
    private GameObject mainCamera;

    [SerializeField]
    [Tooltip("Debug | Current head state")]
    private HeadState headState;

    [SerializeField]
    [Tooltip("Check if the player has been scored on current state")]
    private bool isScored;

    void Update()
    {
        if (mainCamera != Camera.main.gameObject)
        {
            mainCamera = Camera.main.gameObject;
            GenerateCollision(mainCamera, headColliderRadius);
        }
        if (mainCamera == null)
        {
            Debug.LogError("No active camera found in the scene");
            return;
        }

        shortestDistance = ClosestDistance(collidedObjects, mainCamera, headColliderRadius);
        HeadToWaterDistance(shortestDistance);
        HeadCollisionState(shortestDistance);
        HeadScoring();
    }
    /// <summary>
    /// Return the closest distance between two gameobjects in given a list of gameobjects. 
    /// </summary>
    /// <param name="objectList">List of gameobjects to check distance against</param>
    /// <param name="mainObject">The main object which will check against the list of objects</param>
    /// <param name="maxRadius">The max radius the returned distance will revert to if there are no objects in the list</param>
    /// <returns>Float of the shortest distance found between a list of gameobjects to the main object</returns>
    float ClosestDistance(List<GameObject> objectList, GameObject mainObject, float maxRadius)
    {
        float shortDistance;
        if (objectList.Count > 0)
        {
            List<float> distances = new List<float>();
            foreach (GameObject collidedObject in objectList)
            {
                Vector3 closestPoint = collidedObject.GetComponent<Collider>().ClosestPoint(mainObject.transform.position);
                float distance = Vector3.Distance(mainObject.transform.position, closestPoint);
                distances.Add(distance);
            }
            shortDistance = distances.Min();
        }
        else
            shortDistance = maxRadius;
        

        return shortDistance;
    }

    /// <summary>
    /// Given a gameobject and radius, create a sphere collider on it. 
    /// </summary>
    /// <param name="collisionObject"></param>
    void GenerateCollision(GameObject collisionObject, float radius)
    {
        SphereCollider headCollider = collisionObject.AddComponent<SphereCollider>();
        headCollider.isTrigger = true;
        headCollider.center = Vector3.zero;
        headCollider.radius = radius;
    }

    /// <summary>
    /// Sets scoring values based on head collision movement conditions
    /// </summary>
    void HeadScoring()
    {
        switch (headState)
        {
            case HeadState.Warning:
                // Check if warning state has already been scored
                if (!isScored)
                {
                    WaveScore.WarningAmt += 1;
                    isScored = true;
                }
                WaveScore.WarningTime += Time.deltaTime;
                break;
        }
    }
    /// <summary>
    /// Set the HeadState based on distance values from wave
    /// </summary>
    /// <param name="distance"></param>
    void HeadCollisionState(float distance)
    {
        if (distance <= 0.1f) // Fallen
        {
            using (var e = GameLost.Get())
                e.cause = "Head collided with barrel";

            headState = HeadState.Fallen;

            return;
        }
        else if (distance <= 0.5f)
        {
            headState = HeadState.Warning;
            return;
        }
        else
            headState = HeadState.Normal;
    }
    /// <summary>
    /// Given a distance, send water-screen event a scaled value from 0 to 1. 
    /// </summary>
    /// <param name="distance"></param>
    void HeadToWaterDistance(float distance)
    {
        float alphaValue;

        if (distance < headColliderRadius)
            alphaValue = headColliderRadius - distance; // Example: 1 -> 0 becomes 0 -> 1
        else
            alphaValue = 0;

        float scaledValue = Mathf.InverseLerp(0, headColliderRadius, alphaValue);

        using (var e = WaterScreenEvent.Get())
            e.alphaValue = scaledValue;
    }
    /// <summary>
    /// Check for enter collision with water and add detected collided parts
    /// </summary>
    /// <param name="other"></param>
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
    /// <summary>
    /// Check for exit collision with water and remove detected collided parts
    /// </summary>
    /// <param name="other"></param>
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
    public enum HeadState
    {
        Normal,
        Warning,
        Fallen
    }
}
