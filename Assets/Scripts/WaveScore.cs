using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveScore : MonoBehaviour
{
    [SerializeField] Collider winCollider;

    [SerializeField] Transform waveTransform;

    public Animator transition;

    [SerializeField] BoardController board;

    [SerializeField] [FoldoutGroup("Game Won")] GameObject winObject;
    [SerializeField] [FoldoutGroup("Game Won")] TextMeshPro winDataText;

    [SerializeField] [FoldoutGroup("Game Lost")] GameObject loseObject;

    [Tooltip("Starting time the player has to get ready before being monitored")]
    public float startTime;

    [Tooltip("During warmup the player will not be measured or penalised")]
    private static bool warmup = true;

    [SerializeField]
    [Tooltip("Time in seconds it takes for the transition to occur")]
    private GameObject sceneTransition;

    [SerializeField]
    [Tooltip("Gameobject which contains the position for the player to teleport too when fallen")]
    private GameObject failureLocation;

    int warningAmt;
    float warningTime;
    Vector3 headTilt;

    public static GameState State { get; private set; }

    public static bool IsPlaying { get => WaveScore.State == GameState.Playing; }
    public static bool IsWarmup { get => WaveScore.warmup == true; }

    void Awake()
    {
        State = GameState.Playing;
        StartCoroutine(WarmupWaitTime());
        sceneTransition.SetActive(false);
    }

    void Start()
    {
        if (waveTransform == null)
            Debug.LogError("Ensure that WaveScore.waveTransform has the wave's transform assigned");
    }

    void OnEnable()
    {
        WaveSettingEvent.RegisterListener(OnWaveSettingsEvent);
        WaveEndEvent.RegisterListener(OnWaveEndEvent);
        ScoreControlEvent.RegisterListener(OnScoreControlEvent);
        VisualControlEvent.RegisterListener(OnVisualControlEvent);
        GameWon.RegisterListener(OnGameWon);
        GameLost.RegisterListener(OnGameLost);
        VRButtonEvent.RegisterListener(OnVRButtonEvent);
    }

    void OnDisable()
    {
        WaveSettingEvent.UnregisterListener(OnWaveSettingsEvent);
        WaveEndEvent.UnregisterListener(OnWaveEndEvent);
        ScoreControlEvent.UnregisterListener(OnScoreControlEvent);
        VisualControlEvent.UnregisterListener(OnVisualControlEvent);
        GameWon.UnregisterListener(OnGameWon);
        GameLost.UnregisterListener(OnGameLost);
        VRButtonEvent.UnregisterListener(OnVRButtonEvent);
    }

    void Update()
    {
        MovementState moveState = HeadMovement.HeadTiltToState(headTilt);
        if (moveState == MovementState.Fallen && IsPlaying && !IsWarmup)
        {
            State = GameState.Lost;
            StartCoroutine(SplashTransition()); /* Rest In Peace, ocean man :( */
        }

        //Testing code to stop board at any point
        if (Input.GetKey(KeyCode.I))
        {
            board.InputAccepted = false;
            board.StopImmediately();
        }
    }

    /// <summary>
    /// Determins if the player is within the warmup period and sets warmup variable to false when time is exceeded
    /// </summary>
    /// <returns>Null if timer has not been reached or exceeded</returns>
    IEnumerator WarmupWaitTime()
    {
        float startClock = 0.0f;

        while (startClock <= startTime)
        {
            startClock += Time.deltaTime;
            yield return null;
        }
        warmup = false;
    }

    void OnWaveSettingsEvent(WaveSettingEvent e) =>
        winCollider.transform.position = waveTransform.position + (waveTransform.right * e.settings.length);

    void OnScoreControlEvent(ScoreControlEvent e)
    {
        warningAmt = e.warningAmt;
        warningTime = e.warningTime;
    }

    void OnVisualControlEvent(VisualControlEvent e) => headTilt = e.dir;

    void OnWaveEndEvent(WaveEndEvent e)
    {
        // We have won
        if (!IsPlaying)
            return;

        // It isn't exactly necessary to use a separate win event, it's more
        // in the *event* that we need to add more specific data
        // regarding the win that we don't get from the WaveEndEvent data -
        // it's a little bit of future proofing if you will.
        using (var winE = GameWon.Get())
        {
            winE.warningAmt = warningAmt;
            winE.warningTime = warningTime;
        }
    }

    IEnumerator SplashTransition()
    {
        sceneTransition.SetActive(true);

        // Play Animation
        transition.SetTrigger("Splash");

        // Wait
        yield return new WaitForSeconds(1.3f);

        //Teleport Player to custom end location
        board.Motor.SetPosition(failureLocation.transform.position);
        board.StopImmediately();
        board.InputAccepted = false;

        // We have lost so load scene & teleport player
        using (var e = GameLost.Get()) { /* Rest In Peace, ocean man :( */ }

        // Wait
        yield return new WaitForSeconds(0.5f);
        sceneTransition.SetActive(false);
    }

    void OnGameWon(GameWon e)
    {
        // We pass the event's fields rather than the entire event itself as we should not copy the
        // singleton event instance past a single invocation
        State = GameState.Won;
        StartCoroutine(SetupWonMenu(e.warningAmt, e.warningTime));
    }

    // We have to defer setting up the win screen to an IEnumerator as the player continues the move after winning,
    // so we have to wait for the player to stop moving before setting up the win menu
    IEnumerator SetupWonMenu(int warningAmt, float warningTime)
    {
        yield return new WaitUntil(() => board.Motor.Velocity.magnitude <= 0.1f);

        winObject.transform.position = board.transform.position.WithY(n => n + 1.0f);
        winObject.SetActive(true);
        //--- More human-friendly format ---//
        // winDataText.text = $"You were warned {e.warningAmt} times and spend {e.warningTime} seconds in a warning state";
        //--- Simple statistics listing ---//
        winDataText.text = $"Warned: {warningAmt} times\nWarn Time: {warningTime}s";
        board.StopImmediately();
        board.InputAccepted = false;

    }

    void OnGameLost(GameLost e)
    {
        loseObject.transform.position = board.transform.position.WithY(n => n + 1.0f);
        loseObject.SetActive(true);
        board.StopImmediately();
        board.InputAccepted = false;
    }


    void OnVRButtonEvent(VRButtonEvent e)
    {
        switch (e.button)
        {
            case VRButtons.MainMenu: OnMainMenuButton(); break;
            case VRButtons.Retry: OnRetryButton(); break;
            case VRButtons.Designer: OnBarrelDesignerButton(); break;
            default: break; // Ignore all other buttons
        }
    }

    public void OnMainMenuButton() => SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
    public void OnRetryButton() => SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    public void OnBarrelDesignerButton() => SceneManager.LoadSceneAsync("BarrelDesigner", LoadSceneMode.Single);
}

public enum GameState
{
    Playing,
    Won,
    Lost
}