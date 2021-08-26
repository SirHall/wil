using System.Collections;
using System.Collections.Generic;
using Excessives.Unity;
using UnityEngine;

public class WaveDirectionArrow : MonoBehaviour
{
    [SerializeField] float arrowSineDist = 2.0f;
    [SerializeField] float arrowSineSpeed = 0.5f;
    [SerializeField] Transform arrow;

    float arrowInitZ = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        arrowInitZ = arrow.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        arrow.localPosition = arrow.localPosition.WithZ(arrowInitZ +
            (Utils.nSin(Time.time * arrowSineSpeed) * arrowSineDist)
        );
    }


}
