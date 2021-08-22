using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] UnityEvent<Collision> CollisionEnter = new UnityEvent<Collision>();
    [SerializeField] UnityEvent<Collision> CollisionStay = new UnityEvent<Collision>();
    [SerializeField] UnityEvent<Collision> CollisionExit = new UnityEvent<Collision>();


    void OnCollisionEnter(Collision collision) => CollisionEnter.Invoke(collision);
    void OnCollisionStay(Collision collision) => CollisionStay.Invoke(collision);
    void OnCollisionExit(Collision collision) => CollisionExit.Invoke(collision);
}
