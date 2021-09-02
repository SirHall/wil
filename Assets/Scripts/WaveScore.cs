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
    [SerializeField] GameObject introPoints;

    [SerializeField] Transform waveTransform;

    public Animator transition;

    [Tooltip("The max score the player will be ranked off")]
    public int maxScore = 1000;

    [SerializeField] BoardController board;

    [SerializeField] [FoldoutGroup("Game Won")] GameObject winObject;
    [SerializeField] [FoldoutGroup("Game Won")] TextMeshPro winDataText;

    [SerializeField] [FoldoutGroup("Game Won")] TextMeshPro winDataWarningText;
    [SerializeField] [FoldoutGroup("Game Won")] TextMeshPro winDataWarningTimeText;
    [SerializeField] [FoldoutGroup("Game Won")] TextMeshPro winDataScoreText;

    [SerializeField] [FoldoutGroup("Game Lost")] GameObject loseObject;
    [SerializeField] [FoldoutGroup("Game Lost")] TextMeshPro loseDataWarningText;
    [SerializeField] [FoldoutGroup("Game Lost")] TextMeshPro loseDataWarningTimeText;
    [SerializeField] [FoldoutGroup("Game Lost")] TextMeshPro loseDataScoreText;
    [SerializeField] [FoldoutGroup("Game Lost")] TextMeshPro loseDataCauseText;

    [SerializeField] [FoldoutGroup("Game Start")] GameObject startObject;
    [SerializeField] [FoldoutGroup("Game Start")] TextMeshPro startDataText;

    [Tooltip("Starting time the player has to get ready before being monitored")]
    public float startTime;

    [Tooltip("During warmup the player will not be measured or penalised")]
    private static bool warmup = false;

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
        sceneTransition.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(WarmupWaitTime());
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
        if (IsPlaying)
        {
            //Testing code to stop board at any point
            if (Input.GetKey(KeyCode.I))
            {
                board.InputAccepted = false;
                board.StopImmediately();
            }
        }

    }

    /// <summary>
    /// Determins if the player is within the warmup period and sets warmup variable to false when time is exceeded
    /// </summary>
    /// <returns>Null if timer has not been reached or exceeded</returns>
    IEnumerator WarmupWaitTime()
    {
        float startClock = 0.0f;
        warmup = true;

        yield return new WaitForSeconds(0.1f);
        startObject.transform.position = board.transform.position.WithY(n => n + 1.0f);
        startObject.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        while (startClock <= startTime)
        {
            startClock += Time.deltaTime;
            int countDownClock = (int)(startTime - startClock);

            startDataText.text = countDownClock.ToString();
            yield return null;
        }
        startObject.SetActive(false);
        warmup = false;
    }

    void OnWaveSettingsEvent(WaveSettingEvent e)
    {
        Vector3 waveRight = waveTransform.position;
        Vector3 waveLeft = waveTransform.position + (waveTransform.right * e.settings.length);

        bool isRight = e.settings.surfDir == RightLeft.Right;

        winCollider.transform.position = isRight ? waveLeft : waveRight;
        introPoints.transform.position = isRight ? waveRight : waveLeft;
        introPoints.transform.RotateAround(introPoints.transform.position, Vector3.up, isRight ? 0.0f : 180.0f);
        winObject.transform.RotateAround(winCollider.transform.position, Vector3.up, isRight ? 0.0f : 180.0f);
        loseObject.transform.RotateAround(winCollider.transform.position, Vector3.up, isRight ? 0.0f : 180.0f);
        startObject.transform.RotateAround(board.transform.position.WithY(n => n + 1.0f), Vector3.up, isRight ? 0.0f : 180.0f);
    }

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

    IEnumerator SplashTransition(int warningAmt, float warningTime, int maxScore, string causeOfFall)
    {
        sceneTransition.SetActive(true);
        sceneTransition.GetComponentInChildren<Canvas>().worldCamera = Camera.main;

        // Play Animation
        transition.SetTrigger("Splash");

        // Wait
        yield return new WaitForSeconds(1.3f);

        //Teleport Player to custom end location
        board.Motor.SetPosition(failureLocation.transform.position);

        loseDataWarningText.text = $"{warningAmt}";
        loseDataWarningTimeText.text = warningTime.ToString("F2") + "s";
        loseDataCauseText.text = causeOfFall;

        board.StopImmediately();
        board.InputAccepted = false;

        // We have lost so load scene & teleport player
        loseObject.transform.position = board.transform.position.WithY(n => n + 1.0f);
        loseObject.SetActive(true);
        board.StopImmediately();
        board.InputAccepted = false;

        // Wait
        yield return new WaitForSeconds(0.5f);
        sceneTransition.SetActive(false);
    }

    void OnGameWon(GameWon e)
    {
        // We pass the event's fields rather than the entire event itself as we should not copy the
        // singleton event instance past a single invocation
        State = GameState.Won;

        StartCoroutine(SetupWonMenu(e.warningAmt, e.warningTime, maxScore));
    }

    // We have to defer setting up the win screen to an IEnumerator as the player continues the move after winning,
    // so we have to wait for the player to stop moving before setting up the win menu
    IEnumerator SetupWonMenu(int warningAmt, float warningTime, int maxScore)
    {
        yield return new WaitUntil(() => board.Motor.Velocity.magnitude <= 1.2f);

        winObject.transform.position = board.transform.position.WithY(n => n + 1.0f);
        winObject.SetActive(true);
        //--- More human-friendly format ---//
        // winDataText.text = $"You were warned {e.warningAmt} times and spend {e.warningTime} seconds in a warning state";
        //--- Simple statistics listing ---//

        int currentScore = FinalScore(warningAmt, warningTime, maxScore);

        winDataWarningText.text = $"{warningAmt}";
        winDataWarningTimeText.text = warningTime.ToString("F2") + "s";
        // Need to implement dynamic score based on warnings and performance. 
        // Ideas: Max score of 1000. All errors reduce current score (starts at max score). Essentially a surf without any errors will result in the max score. 
        winDataScoreText.text = $"{currentScore}";
        board.StopImmediately();
        board.InputAccepted = false;

    }

    /// <summary>
    /// Returns an adjusted int score when given the number of errors, length they have occured, and total maxScore.
    /// </summary>
    /// <param name="errorAmt"></param>
    /// <param name="errorTime"></param>
    /// <param name="maxScore"></param>
    /// <returns>Int value with new clamped score between 0 and 1000</returns>
    int FinalScore(int errorAmt, float errorTime, int maxScore)
    {
        float correctedTime = errorTime + 1f; // Always add 1 second to warning time to increase errorScore results from time. 
        int correctedScore = (int)(((float)errorAmt * correctedTime) * 20); // ErrorScore based off warning time and warning amount. Score is then multipled to return a much larger value. 
        int clampedErrorScore = Mathf.Clamp(correctedScore, 0, maxScore); // ErrorScore is clamped to be between the 0 and the maxScore

        int currentScore = maxScore - clampedErrorScore; // Finished score to display on UI. 

        return currentScore;
    }

    void OnGameLost(GameLost e)
    {
        if (!IsPlaying)
            return;
        State = GameState.Lost;
        StartCoroutine(SplashTransition(warningAmt, warningTime, maxScore, e.cause)); /* Rest In Peace, ocean man :( */
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