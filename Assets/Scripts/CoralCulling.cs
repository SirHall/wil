using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralCulling : MonoBehaviour
{
    private GameObject player;

    // All coral contained within a parent game object
    private List<GameObject> coralList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Add all coral within parent gameobject to coralList
        foreach (Transform child in transform)
        {
            coralList.Add(child.gameObject);
        }
    }

    void LateUpdate()
    {
        // For each coral in coralList check distance from player. Disable if distance is too far away to save resources. 
        foreach (GameObject coral in coralList)
        {
            float distance = Vector3.Distance(coral.transform.position, player.transform.position);
            if (distance >= 30)
            {
                coral.gameObject.SetActive(false);
            }
            else
            {
                coral.gameObject.SetActive(true);
            }
        }
    }
}
