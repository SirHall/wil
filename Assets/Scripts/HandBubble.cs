using System.Collections;
using System.Collections.Generic;
using UnityConstantsGenerator;
using UnityEngine;

public class HandBubble : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    void Start()
    {
        particle.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<InteractionType>() == null) return;

        if (c.GetComponent<InteractionType>().interactable == Interactables.Water)
        {
            if (!particle.gameObject.activeInHierarchy)
                particle.gameObject.SetActive(true);
        }
            
    }
    void OnTriggerExit(Collider c)
    {
        if (c.GetComponent<InteractionType>() == null) return;

        if (c.GetComponent<InteractionType>().interactable == Interactables.Water)
            particle.gameObject.SetActive(false);
    }

}
