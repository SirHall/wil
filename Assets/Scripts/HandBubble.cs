using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBubble : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    int activate = 0;

    void Update()
    {
        particle.gameObject.SetActive(activate > 0);
        if (activate > 0)
            activate--;
    }

    void OnTriggerStay(Collider c)
    {

        activate = 3;
    }
}
