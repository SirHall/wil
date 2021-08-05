using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour{

    //Sounds
    [SerializeField] public AudioClip Splash;
    [SerializeField] public AudioClip Underwater;
    [SerializeField] public AudioClip Idle;
    [SerializeField] public AudioClip Background;
    [SerializeField] public AudioClip Moving;
    [SerializeField] public AudioClip Inwave;


    //Creating an instance for other files control
    public static SoundManager instance;
    public static AudioClip aSplash { get => SoundManager.instance.Splash; }
    public static AudioClip aUnderwater { get => SoundManager.instance.Underwater; }

    //settings
    [Range(10,100)]
    [SerializeField] public int MovementMaxVolume = 100;
    [SerializeField] AudioSource backgroundsource1;
    [SerializeField] AudioSource backgroundsource2;





    //A class that is called upon from any other file
    //Example of referencing in other parts of the code
    //SoundManager.Playsound(SoundManager.instance.splash);
    //it plays all of the sound and doesn't stop

    public static void Playsound(AudioClip sound){
        GameObject soundmanager = GameObject.Find("SoundManager");
        AudioSource audioSource = soundmanager.AddComponent<AudioSource>();
        audioSource.clip = sound;
        audioSource.PlayOneShot(sound);
    }

    private void Awake(){
        instance = this;

        backgroundsource1 = gameObject.AddComponent<AudioSource>();
        backgroundsource2 = gameObject.AddComponent<AudioSource>();

        boardcontroller = GameObject.FindObjectOfType<BoardController>();
    }

    void Start(){

        Playsound(Splash);
    }      

    //wave stuff
    int range = 4;
    Vector3 Wavepos;
    Vector3 Boardpos;



    //for board
    public BoardController boardcontroller;
    float Total_Velocity = 0;


    void Update(){
        Total_Velocity = Mathf.Abs(boardcontroller.Motor.BaseVelocity.x) + Mathf.Abs(boardcontroller.Motor.BaseVelocity.z);
        //set velocity to set range between 1-10
        if (Total_Velocity > 10){
            Total_Velocity = 10;
        }
        else if (Total_Velocity < 1){
            Total_Velocity = 0;
        }
        Total_Velocity = (int)Total_Velocity;

        //Playsound here
        if (!backgroundsource1.isPlaying) {
            backgroundsource1.clip = Background;
            backgroundsource1.loop = true;
            backgroundsource1.Play();
        }

        if (!backgroundsource2.isPlaying)
        {
            backgroundsource2.clip = Moving;
            backgroundsource2.loop = true;
            backgroundsource2.Play();
        }

        backgroundsource2.volume = (Total_Velocity * Mathf.Sin(Mathf.PI / 2) / 10)/100 * MovementMaxVolume;

        


   
    }

}
