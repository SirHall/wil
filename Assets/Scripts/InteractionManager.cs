using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can active and send out triggers for when the player touches objects.
/// Triggers can include Audio, Bool values, Coordinates.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    private bool isInteractable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        using (var e = InteractionControlEvent.Get())
            e.input.isInteracting = isInteractable;
    }

    void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.layer == 6) 
        {
            isInteractable = true;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == 6) {
            isInteractable = false;
        }
    }
}
