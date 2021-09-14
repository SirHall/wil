using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameplaySettings : MonoBehaviour
{
    public static GameplaySettings Instance { get; private set; }

    // --- Bobbing setting ---
    [ShowInInspector] public bool Bobbing { get; set; } = true; //Enables the board bobbing

    // --- Warmup setting ---
    [ShowInInspector] public float WarmupTime { get; set; } = 5f; //Sets the length of time the warmup will last for

    // --- Intro setting ---
    [ShowInInspector] public bool IntroStart { get; set; } = true; //Enables or disables the starting intro

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void EmitGameplaySettings()
    {
        if (Instance == null)
            return;

        using (var e = GameplaySettingEvent.Get())
        {
            e.settings = new GameSettings
            {
                bobbing = Instance.Bobbing,
                warmupTime = Instance.WarmupTime,
                introStart = Instance.IntroStart
            };
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
