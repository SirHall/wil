using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoralCulling : MonoBehaviour
{

    [Tooltip("The distance within which the coral is enabled")]
    [SerializeField] float seeDist = 30.0f;

    [SerializeField] GameObject player;

    // All coral contained within a parent game object
    private List<GameObject> coralList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Add all coral within parent gameobject to coralList
        foreach (Transform child in transform)
            coralList.Add(child.gameObject);
    }

    void LateUpdate()
    {
        // For each coral in coralList check distance from player. Disable if distance is too far away to save resources. 
        foreach (GameObject coral in coralList)
        {
            bool visible = Vector3.Distance(coral.transform.position, player.transform.position) <= seeDist;
            if (coral.activeSelf != visible) // Only update active state if it needs to be changes
                coral.gameObject.SetActive(visible);
        }
    }
}
