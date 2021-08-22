using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] UnityEvent<Collision> CollisionEnter;
    [SerializeField] UnityEvent<Collision> CollisionStay;
    [SerializeField] UnityEvent<Collision> CollisionExit;


    void OnCollisionEnter(Collision collision) => CollisionEnter.Invoke(collision);
    void OnCollisionStay(Collision collision) => CollisionStay.Invoke(collision);
    void OnCollisionExit(Collision collision) => CollisionExit.Invoke(collision);
}
