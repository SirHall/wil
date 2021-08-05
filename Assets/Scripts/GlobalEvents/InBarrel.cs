using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBarrel : MonoBehaviour
{

    int range = 4;
    Vector3 Wavepos;
    Vector3 Boardpos;


    void Update() {

        var pos = GameObject.Find("Wave").transform.position;
        var WaveObj = GameObject.Find("Wave");
        var BoardObj = GameObject.Find("Surfboard");
        //if they exist
        if (WaveObj && BoardObj)
        {
            Wavepos = new Vector3(WaveObj.transform.position.x, WaveObj.transform.position.y, WaveObj.transform.position.z);
            Boardpos = new Vector3(BoardObj.transform.position.x, BoardObj.transform.position.y, BoardObj.transform.position.z);
            

            if (Vector3.Distance(Wavepos, Boardpos) < range)
            {
                Debug.Log("InRange");
               
            }

            //pos.x,pos.y,pos.z
        }

    }
    



    
}
