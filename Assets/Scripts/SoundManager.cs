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
    [SerializeField] private AudioClip Grab;

    [SerializeField]
    private SoundLevelMode currentSoundLevelState;

    [SerializeField]
    private EnvironmentMode currentEnvironmentState;

    //Creating an instance for other files control
    public static SoundManager instance;
    public static AudioClip aSplash { get => SoundManager.instance.Splash; }
    public static AudioClip aUnderwater { get => SoundManager.instance.Underwater; }

    //settings
    [Range(10,100)]
    [SerializeField] public int MovementMaxVolume = 100;
    [SerializeField] AudioSource backgroundsource1;
    [SerializeField] AudioSource backgroundsource2;
    [SerializeField] AudioSource leanWarningSource;
    
    private AudioSource grabSource;

    private Vector3 headPos = new Vector3();

    private bool isFallen = false;

    void OnEnable() {
        SoundControlEvent.RegisterListener(SoundEvent);
        GripInteraction.RegisterListener(GrabSound);
    }

    void OnDisable() {
        SoundControlEvent.UnregisterListener(SoundEvent);
        GripInteraction.RegisterListener(GrabSound);
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
        grabSource = gameObject.AddComponent<AudioSource>();

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

        bool playingMusic;


        if (WaterStateChanged())
        {
            if (currentEnvironmentState == EnvironmentMode.UnderWater) backgroundsource1.clip = Underwater;
            else if (currentEnvironmentState == EnvironmentMode.AboveWater) backgroundsource1.clip = Background;

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
        FallenSound();
    }
    /// <summary>
    /// Handles the Lean warning sound which plays when the user is too far off the board
    /// </summary>
    private void LeanWarningSound() 
    {
        if (!WaveScore.IsPlaying) return;

        if (LeanStateChanged()) 
        {
            leanWarningSource.clip = Woah;
            switch (currentSoundLevelState) 
            {
                case SoundLevelMode.Quite:
                    leanWarningSource.volume = 0.4f;
                    break;
                case SoundLevelMode.Warning:
                    leanWarningSource.volume = 0.6f;
                    break;
                case SoundLevelMode.Alarm:
                    leanWarningSource.volume = 1f;
                    break;
            }
            if (currentSoundLevelState == SoundLevelMode.Quite || currentSoundLevelState == SoundLevelMode.Warning || currentSoundLevelState == SoundLevelMode.Alarm)
                leanWarningSource.Play();
        }
    }

    private void GrabSound(GripInteraction e)
    {
        grabSource.clip = Grab;
        grabSource.volume = 100f;
        grabSource.Play();
    }

    private void FallenSound()
    {
        if (WaveScore.IsPlaying)
        {
            isFallen = false;
            return;
        }

        if (!isFallen)
        {
            PlaySound(Splash, 0.5f);
            isFallen = true;
        }

    }

    /// <summary>
    /// Takes in the head tilt and returns it as a sound mode
    /// </summary>
    /// <param name="headTilt">Vector 3 value for the current VR head tilt coordinates</param>
    /// <returns>SoundLevelMode Enum corresponding to the heads positional distance from the center of the board</returns>
    public static SoundLevelMode HeadPosToSoundMode(Vector3 headTilt)
    {
        // The scalar euclidean distance the head has moved from its original position 
        float headPosDist = Mathf.Max(Mathf.Abs(headTilt.z), Mathf.Abs(headTilt.x));

        if (headPosDist >= 1.0f) 
            return SoundLevelMode.Alarm;
        else if (headPosDist >= 0.8)
            return SoundLevelMode.Warning;
        else if (headPosDist >= 0.6f) 
            return SoundLevelMode.Quite;
        else
            return SoundLevelMode.None;
    }
    /// <summary>
    /// Sets the sound state based on the heads distance from the center of the board
    /// </summary>
    private bool LeanStateChanged() 
    {
        SoundLevelMode previousState = currentSoundLevelState;
        currentSoundLevelState = HeadPosToSoundMode(headPos);
        if (previousState != currentSoundLevelState) return true;
        else return false;
    }

    /// <summary>
    /// Sets the sound state based on if the head is above or below the water and returns a bool if the state has changed
    /// </summary>
    /// <returns>Bool value if environment state has changed or not</returns>
    private bool WaterStateChanged()
    {
        EnvironmentMode previousState = currentEnvironmentState;

        if (headPos.y <= 0) currentEnvironmentState = EnvironmentMode.UnderWater;
        else currentEnvironmentState = EnvironmentMode.AboveWater;

        if (previousState != currentEnvironmentState) return true;
        else return false;
    }

    public enum SoundLevelMode 
    {
        None,
        Quite,
        Warning,
        Alarm
    }

    public enum EnvironmentMode
    {
        None,
        AboveWater,
        UnderWater
    }
}
