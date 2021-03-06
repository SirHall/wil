using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour{

    //Sounds
    [Tooltip("Splashing into the ocean")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Splash;
    [Tooltip("Underwater ambience")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Underwater;
    [Tooltip("Above water ambience")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Background;
    [Tooltip("Surfboard moving")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Moving;
    [Tooltip("In barrel ambience")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Inwave;
    [Tooltip("Falling off warning")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Woah;
    [Tooltip("Grabbing the board")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip Grab;
    [Tooltip("Touching the barrel wave")]
    [SerializeField] [FoldoutGroup("Game Sounds")] private AudioClip BarrelTouch;

    [SerializeField]
    [Tooltip("Master volume which controls all sounds within the application")]
    private float masterVolume = 1;

    [Tooltip("Current audio level state the sounds should be played in")]
    private SoundLevelMode currentSoundLevelState;

    [Tooltip("Current environment the player is in")]
    private EnvironmentMode currentEnvironmentState;

    //Creating an instance for other files control
    public static SoundManager instance;
    public static AudioClip aSplash { get => SoundManager.instance.Splash; }
    public static AudioClip aUnderwater { get => SoundManager.instance.Underwater; }

    //settings
    [Range(10,100)]
    public int MovementMaxVolume = 100;

    private AudioSource backgroundsource1;
    private AudioSource backgroundsource2;
    private AudioSource leanWarningSource;
    private AudioSource grabSource;
    private AudioSource barrelTouchSource;

    private Vector3 headPos = new Vector3();

    private bool isFallen = false;

    //for board
    public BoardController boardcontroller;
    float Total_Velocity = 0;

    void OnEnable() {
        SoundControlEvent.RegisterListener(SoundEvent);
        GripInteraction.RegisterListener(GrabSound);
        WaveInteraction.RegisterListener(BarrelTouchSound);
        GameSettingsEvent.RegisterListener(OnGameplaySettingEvent);
    }

    void OnDisable() {
        SoundControlEvent.UnregisterListener(SoundEvent);
        GripInteraction.UnregisterListener(GrabSound);
        WaveInteraction.UnregisterListener(BarrelTouchSound);
        GameSettingsEvent.UnregisterListener(OnGameplaySettingEvent);
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
    void OnGameplaySettingEvent(GameSettingsEvent e)
    {
        masterVolume = e.audioSettings.audioLevel;
    }

    void MasterAudioVolume()
    {
        if (AudioListener.volume != masterVolume)
            AudioListener.volume = Mathf.Clamp(Mathf.InverseLerp(0, 100, masterVolume), 0, 1);
    }

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
        barrelTouchSource = gameObject.AddComponent<AudioSource>();

        boardcontroller = GameObject.FindObjectOfType<BoardController>();
    }

    void Start()
    {
        PlaySound(Splash, 0.5f);
    }      

    void Update()
    {
        Total_Velocity = Mathf.Abs(boardcontroller.Motor.BaseVelocity.x) + Mathf.Abs(boardcontroller.Motor.BaseVelocity.z);
        //set velocity to set range between 1-10
        if (Total_Velocity > 10)
            Total_Velocity = 10;
        
        else if (Total_Velocity < 1)
            Total_Velocity = 0;

        Total_Velocity = (int)Total_Velocity;

        //Playsound here

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
        MasterAudioVolume();
        LeanWarningSound();
        FallenSound();
    }

    private void BarrelTouchSound(WaveInteraction e)
    {
        if(e.isTouching && !barrelTouchSource.isPlaying)
        { 
            barrelTouchSource.clip = BarrelTouch;
            barrelTouchSource.loop = true;
            barrelTouchSource.volume = 0.6f;
            barrelTouchSource.Play();
        }
        else if (!e.isTouching && barrelTouchSource.isPlaying)
        {
            barrelTouchSource.Stop();
        }
    }

    /// <summary>
    /// Handles the Lean warning sound which plays when the user is too far off the board
    /// </summary>
    private void LeanWarningSound() 
    {
        if (!WaveScore.IsPlaying || WaveScore.IsWarmup) return;

        if (LeanStateChanged()) 
        {
            leanWarningSource.clip = Woah;
            switch (currentSoundLevelState) 
            {
                case SoundLevelMode.Quiet:
                    leanWarningSource.volume = 0.4f;
                    break;
                case SoundLevelMode.Warning:
                    leanWarningSource.volume = 0.6f;
                    break;
                case SoundLevelMode.Alarm:
                    leanWarningSource.volume = 1f;
                    break;
            }
            if (currentSoundLevelState == SoundLevelMode.Quiet || currentSoundLevelState == SoundLevelMode.Warning || currentSoundLevelState == SoundLevelMode.Alarm)
                leanWarningSource.Play();
        }
    }

    /// <summary>
    /// When triggered will play the grab sound effect once
    /// </summary>
    /// <param name="e"></param>
    private void GrabSound(GripInteraction e)
    {
        grabSource.clip = Grab;
        grabSource.volume = 0.9f;
        grabSource.Play();
    }

    /// <summary>
    /// Plays the fall sound effect once
    /// </summary>
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
            return SoundLevelMode.Quiet;
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
        Quiet,
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
