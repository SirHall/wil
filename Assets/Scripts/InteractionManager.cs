using System.Collections;
using System.Collections.Generic;
using UnityConstantsGenerator;
using UnityEngine;

/// <summary>
/// Can active and send out triggers for when the player touches objects.
/// Triggers can include Audio, Bool values, Coordinates.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    [SerializeField] Interactables interactable = Interactables.None;
    private bool isInteractable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteractable)
        {
            using (var e = InteractablesEvent.Get())
                e.interactables = interactable;
        }
    }

    void OnTriggerEnter(Collider collision) 
    {

        if(collision.gameObject.layer == (int)LayerId.Hands) 
        {
            print("I'm interacting with hands");
            isInteractable = true;
        }
    }

    void OnTriggerExit(Collider collision) {
        if (collision.gameObject.layer == (int)LayerId.Hands) {
            isInteractable = false;
        }
    }
}
