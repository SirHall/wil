using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    //Active sounds
    [SerializeField] public AudioClip splash;
    [SerializeField] public AudioClip underwater;

    //Creating an instance for other files control
    public static SoundManager instance;
    public static AudioClip Splash { get => SoundManager.instance.splash; }
    public static AudioClip Underwater { get => SoundManager.instance.underwater; }



    //background sounds
    [SerializeField] public AudioClip idle;
    [SerializeField] public AudioClip notidle;
    [SerializeField] public AudioClip moving;
    [SerializeField] public AudioClip inwave;



    //settings
    [SerializeField] public int stance = 0;
    [SerializeField] public int previous_stance = 1;
    [SerializeField] AudioSource backgroundsource1;
    [SerializeField] AudioSource backgroundsource2;
    public static int Stance { get => SoundManager.instance.stance; }




    //A class that is called upon from any other file
    //Example of referencing in other parts of the code
    //SoundManager.Playsound(SoundManager.instance.splash);
    //it plays all of the sound and doesn't stop
    public static void Playsound(AudioClip sound)
    {
        GameObject soundmanager = GameObject.Find("SoundManager");
        AudioSource audioSource = soundmanager.AddComponent<AudioSource>();
        audioSource.PlayOneShot(sound);
    }



    private void Awake()
    {
        instance = this;

        backgroundsource1 = gameObject.AddComponent<AudioSource>();
        backgroundsource2 = gameObject.AddComponent<AudioSource>();
    }



    void Start()
    {

        Playsound(splash);


    }


    void Update()
    {
        //stance is initially set to idle and can be changed.
        //stance can be change in other files by 
        //SoundManager.instance.stance = 1;
        if (stance != previous_stance)
        {
            switch (stance)
            {
                case 0:

                    backgroundsource1.clip = idle;
                    backgroundsource1.loop = true;
                    backgroundsource1.Play();

                    backgroundsource2.Stop();
                    break;
                case 1:
                    backgroundsource1.clip = notidle;
                    backgroundsource1.loop = true;
                    backgroundsource1.Play();

                    backgroundsource2.clip = moving;
                    backgroundsource2.loop = true;
                    backgroundsource2.Play();

                    break;
                case 2:
                    backgroundsource1.clip = inwave;
                    backgroundsource1.loop = true;
                    backgroundsource1.Play();

                    backgroundsource2.Stop();
                    break;
            }
        }
        previous_stance = stance;
    }
}
