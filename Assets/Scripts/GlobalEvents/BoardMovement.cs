using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMovement : MonoBehaviour
{
    public BoardController boardcontroller;

    float Total_Velocity = 0;

    void Awake()
    {
        boardcontroller = GameObject.FindObjectOfType<BoardController>();
    }

    void Update()
    {
       Total_Velocity = Mathf.Abs(boardcontroller.Motor.BaseVelocity.x) + Mathf.Abs(boardcontroller.Motor.BaseVelocity.z);
       //Debug.Log("reee"+ boardcontroller.Motor.BaseVelocity );
       // Debug.Log("reee" + Total_Velocity);

    }
}
