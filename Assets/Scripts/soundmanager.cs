using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundmanager : MonoBehaviour
{
    bool ran = false;
    int current = 1;
    public AudioSource audioSource;
    public AudioSource audioEffects;
    public AudioClip splash;
    public AudioClip idle;
    public AudioClip moving;
    public AudioClip inwave;
    public AudioClip underwater;

    GameObject board = GameObject.Find("BoardNew");
    // Start is called before the first frame update
    void Start() {

        audioSource = gameObject.AddComponent<AudioSource>();
        audioEffects = gameObject.AddComponent<AudioSource>();

        //only splash on start

        audioEffects.clip = splash;
        audioEffects.Play();
    }


    // Update is called once per frame
    Rigidbody rb = GameObject.Find("Boardnew").GetComponent<Rigidbody>();
    // = board.GetComponent<Rigidbody>();
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            switch (current)
            {
                case 1:
                    audioSource.clip = idle;
                    break;
                case 2:
                    audioSource.clip = moving;
                    break;
                case 3:
                    audioSource.clip = inwave;
                    break;
                case 4:
                    audioSource.clip = underwater;
                    break;

            }            
            
            audioSource.Play();




        }
        else {
            //check if we should add
            
            

        }
       
        
    }
}
