using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{


    [SerializeField] public AudioClip splash;
    [SerializeField] public AudioClip idle;
    [SerializeField] public AudioClip moving;
    [SerializeField] public AudioClip inwave;
    [SerializeField] public AudioClip underwater;

    public static SoundManager instance;



    public static AudioClip Splash { get => SoundManager.instance.splash; }
    public static AudioClip Idle { get => SoundManager.instance.idle; }
    public static AudioClip Moving { get => SoundManager.instance.moving; }
    public static AudioClip Inwave { get => SoundManager.instance.inwave; }
    public static AudioClip Underwater { get => SoundManager.instance.underwater; }



    public static void Playsound(AudioClip sound)
    {
        GameObject soundmanager = GameObject.Find("SoundManager");
        AudioSource audioSource = soundmanager.AddComponent<AudioSource>();
        audioSource.PlayOneShot(sound);
    }

    private void Awake()
    {
        instance = this;
    }

    //Example of referencing in other parts of the code
    //SoundManager.Playsound(SoundManager.instance.idle);

    void Start()
    {
        Playsound(splash);

    }
    void Update()
    {



    }

}
