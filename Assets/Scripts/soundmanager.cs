using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip splash;
    [SerializeField] AudioClip idle;
    [SerializeField] AudioClip moving;
    [SerializeField] AudioClip inwave;
    [SerializeField] AudioClip underwater;

    AudioSource audioSource;
    AudioSource audioEffects;
    GameObject board;
    Rigidbody rb;

    #region Bookkeeping

    bool ran = false;
    int current = 1;

    #endregion

    void Start()
    {
        // Should probably swap out direct references to use the GlobalEvent system
       // board = GameObject.Find("BoardNew");
        //rb = GameObject.Find("Boardnew").GetComponent<Rigidbody>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioEffects = gameObject.AddComponent<AudioSource>();

        //only splash on start
        audioEffects.PlayOneShot(splash);
    }




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
        else
        {
            //check if we should add
        }


    }
}
