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
    private Interactables leftInteractableType;
    private Interactables rightInteractableType;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<HandManager>() == null) return;

        if (gameObject.GetComponent<HandManager>().handType == HandManager.HandType.left)
        {
            using (var e = LeftInteractablesEvent.Get())
                e.leftInteractable = leftInteractableType;
        }
        if (gameObject.GetComponent<HandManager>().handType == HandManager.HandType.right)
        {
            using (var e = RightInteractablesEvent.Get())
                e.rightInteractable = rightInteractableType;
        }
    }

    void OnTriggerEnter(Collider collision) 
    {
        if (collision.GetComponent<InteractionType>() == null || gameObject.GetComponent<HandManager>() == null) return;

        if (gameObject.GetComponent<HandManager>().handType == HandManager.HandType.left)
            leftInteractableType = collision.GetComponent<InteractionType>().interactable;

        if (gameObject.GetComponent<HandManager>().handType == HandManager.HandType.right)
            rightInteractableType = collision.GetComponent<InteractionType>().interactable;
    }

    void OnTriggerExit(Collider collision) 
    {
        if (collision.GetComponent<InteractionType>() == null || gameObject.GetComponent<HandManager>() == null) return;

        if (gameObject.GetComponent<HandManager>().handType == HandManager.HandType.left)
            leftInteractableType = Interactables.None;

        if (gameObject.GetComponent<HandManager>().handType == HandManager.HandType.right)
            rightInteractableType = Interactables.None;
    }
}
