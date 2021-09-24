using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed = 1.0F;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = startPos;
    }

    void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, endPos, ref velocity, speed * Time.deltaTime);
    }
}
