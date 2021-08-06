using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour{

    //Sounds
    [SerializeField] private AudioClip Splash;
    [SerializeField] private AudioClip Underwater;
    [SerializeField] private AudioClip Idle;
    [SerializeField] private AudioClip Background;
    [SerializeField] private AudioClip Moving;
    [SerializeField] private AudioClip Inwave;
    [SerializeField] private AudioClip Woah;

    [SerializeField]
    private SoundMode currentSoundState;

    //Creating an instance for other files control
    public static SoundManager instance;
    public static AudioClip aSplash { get => SoundManager.instance.Splash; }
    public static AudioClip aUnderwater { get => SoundManager.instance.Underwater; }

    //settings
    [Range(10,100)]
    [SerializeField] public int MovementMaxVolume = 100;
    [SerializeField] AudioSource backgroundsource1;
    [SerializeField] AudioSource backgroundsource2;
    [SerializeField] private AudioSource leanWarningSource;

    private Vector3 headPos = new Vector3();

    private bool playSound = false;

    void OnEnable() {
        SoundControlEvent.RegisterListener(SoundEvent);
    }

    void OnDisable() {
        SoundControlEvent.UnregisterListener(SoundEvent);
    }

    // A controller has announced new data
    void SoundEvent(SoundControlEvent e) 
    {
        headPos = e.headInput.dir;
    }

    //A class that is called upon from any other file
    //Example of referencing in other parts of the code
    //SoundManager.Playsound(SoundManager.instance.splash);
    //it plays all of the sound and doesn't stop

    public static void PlaySound(AudioClip sound, float volume)
    {
        GameObject soundmanager = GameObject.Find("SoundManager");
        AudioSource audioSource = soundmanager.AddComponent<AudioSource>();
        audioSource.clip = sound;
        audioSource.volume = volume;
        audioSource.PlayOneShot(sound);
    }

    private void Awake()
    {
        instance = this;

        backgroundsource1 = gameObject.AddComponent<AudioSource>();
        backgroundsource2 = gameObject.AddComponent<AudioSource>();

        leanWarningSource = gameObject.AddComponent<AudioSource>();

        boardcontroller = GameObject.FindObjectOfType<BoardController>();
    }

    void Start()
    {
        PlaySound(Splash, 0.5f);
    }      

    //wave stuff
    int range = 4;
    Vector3 Wavepos;
    Vector3 Boardpos;

    //for board
    public BoardController boardcontroller;
    float Total_Velocity = 0;

    void Update()
    {
        Total_Velocity = Mathf.Abs(boardcontroller.Motor.BaseVelocity.x) + Mathf.Abs(boardcontroller.Motor.BaseVelocity.z);
        //set velocity to set range between 1-10
        if (Total_Velocity > 10)
        {
            Total_Velocity = 10;
        }
        else if (Total_Velocity < 1)
        {
            Total_Velocity = 0;
        }
        Total_Velocity = (int)Total_Velocity;

        //Playsound here
        if (!backgroundsource1.isPlaying) 
        {
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

        LeanWarningSound();
    }
    /// <summary>
    /// Handles the Lean warning sound which plays when the user is too far off the board
    /// </summary>
    private void LeanWarningSound() 
    {
        if (!WaveScore.IsPlaying) return;

        SetState();
        if (playSound) 
        {
            leanWarningSource.clip = Woah;
            switch (currentSoundState) 
            {
                case SoundMode.Quite:
                    leanWarningSource.volume = 0.2f;
                    break;
                case SoundMode.Warning:
                    leanWarningSource.volume = 0.5f;
                    break;
                case SoundMode.Alarm:
                    leanWarningSource.volume = 1f;
                    break;
            }
            if (currentSoundState == SoundMode.Quite || currentSoundState == SoundMode.Warning || currentSoundState == SoundMode.Alarm)
                leanWarningSource.Play();

            playSound = false;
        }
    }

    // This function takes in the head tilt and returns it as a sound mode
    public static SoundMode HeadPosToSoundMode(Vector3 headTilt)
    {
        // The scalar euclidean distance the head has moved from its original position 
        float headPosDist = Mathf.Max(Mathf.Abs(headTilt.z), Mathf.Abs(headTilt.x));

        if (headPosDist >= 1.0f) 
            return SoundMode.Alarm;
        else if (headPosDist >= 0.8)
            return SoundMode.Warning;
        else if (headPosDist >= 0.6f) 
            return SoundMode.Quite;
        else
            return SoundMode.None;
    }
    /// <summary>
    /// Sets the sound state based on the heads distance from the center of the board
    /// </summary>
    void SetState() 
    {
        SoundMode previousState = currentSoundState;
        currentSoundState = HeadPosToSoundMode(headPos);
        if (previousState != currentSoundState)
            playSound = true;
    }

    public enum SoundMode 
    {
        None,
        Quite,
        Warning,
        Alarm
    }
}
