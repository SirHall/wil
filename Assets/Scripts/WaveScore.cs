using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveScore : MonoBehaviour
{
    [SerializeField] Collider winCollider;

    int warningAmt;
    float warningTime;
    Vector3 headTilt;

    public static GameState State { get; private set; }

    public static bool IsPlaying { get => WaveScore.State == GameState.Playing; }

    void Awake()
    {
        State = GameState.Playing;
    }

    void OnEnable()
    {
        WaveSettingEvent.RegisterListener(OnWaveSettingsEvent);
        WaveEndEvent.RegisterListener(OnWaveEndEvent);
        ScoreControlEvent.RegisterListener(OnScoreControlEvent);
        VisualControlEvent.RegisterListener(OnVisualControlEvent);
    }

    void OnDisable()
    {
        WaveSettingEvent.UnregisterListener(OnWaveSettingsEvent);
        WaveEndEvent.UnregisterListener(OnWaveEndEvent);
        ScoreControlEvent.UnregisterListener(OnScoreControlEvent);
        VisualControlEvent.UnregisterListener(OnVisualControlEvent);
    }

    void Update()
    {
        MovementState moveState = HeadMovement.HeadTiltToState(headTilt);
        if (moveState == MovementState.Fallen && IsPlaying)
        {
            State = GameState.Lost;

            StartCoroutine(RunInEndEndScene(() =>
            {
                // We have lost
                using (var e = GameLost.Get()) { /* Rest In Peace, ocean man :( */ }
            }));
        }
    }

    void OnWaveSettingsEvent(WaveSettingEvent e) =>
        winCollider.transform.position = new Vector3(e.settings.length, 0.0f, 0.0f);

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

        State = GameState.Won;
        StartCoroutine(RunInEndEndScene(() =>
        {
            // It isn't exactly necessary to use a separate win event, it's more
            // in the *event* that we need to add more specific data
            // regarding the win that we don't get from the WaveEndEvent data -
            // it's a little bit of future proofing if you will.
            using (var winE = GameWon.Get())
            {
                winE.warningAmt = warningAmt;
                winE.warningTime = warningTime;
            }
        }));


        SceneManager.UnloadSceneAsync("Game");
    }

    IEnumerator RunInEndEndScene(Action action)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("GameEndScreen", LoadSceneMode.Additive);
        load.allowSceneActivation = true;
        while (!load.isDone)
            yield return null;

        Scene endScene = SceneManager.GetSceneByName("GameEndScreen");
        SceneManager.SetActiveScene(endScene);

        action();

        // Comment this out and we have the game end screen over the main game
        // SceneManager.UnloadSceneAsync("Game");
    }
}

public enum GameState
{
    Playing,
    Won,
    Lost
}