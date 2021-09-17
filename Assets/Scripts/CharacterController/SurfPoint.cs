using System.Collections;
using Excessives.Unity;
using System.Collections.Generic;
using UnityEngine;

public class SurfPoint : MonoBehaviour
{
    public static SurfPoint Instance { get; private set; }

    bool bobbing = true;

    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There may only be one instance of SurfPoint at any time");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        GameSettingsEvent.RegisterListener(OnGameSettingsEvent);
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;

        GameSettingsEvent.UnregisterListener(OnGameSettingsEvent);
    }

    void LateUpdate()
    {
        if (BoardController.Instance)
            transform.position = GenY(BoardController.Instance.Motor.TransientPosition);
    }

    Vector3 GenY(Vector3 v) => (bobbing && WaveScore.State != GameState.Lost) ? v.WithY(GFXBoard.Instance.transform.position.y) : v;

    void OnGameSettingsEvent(GameSettingsEvent e)
    {
        bobbing = e.gameplaySettings.bobbing;
    }
}
