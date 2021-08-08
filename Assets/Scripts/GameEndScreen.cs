using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndScreen : MonoBehaviour
{
    public static GameEndScreen Instance { get; private set; }

    [SerializeField] BoardController board;

    [SerializeField] [FoldoutGroup("Game Won")] GameObject winObject;
    [SerializeField] [FoldoutGroup("Game Won")] TextMeshPro winDataText;

    [SerializeField] [FoldoutGroup("Game Lost")] GameObject loseObject;

    void OnEnable()
    {
        Instance = this;
        GameWon.RegisterListener(OnGameWon);
        GameLost.RegisterListener(OnGameLost);
        VRButtonEvent.RegisterListener(OnVRButtonEvent);
    }

    void OnDisable()
    {
        Instance = null;
        GameWon.UnregisterListener(OnGameWon);
        GameLost.UnregisterListener(OnGameLost);
        VRButtonEvent.UnregisterListener(OnVRButtonEvent);
    }

    void OnGameWon(GameWon e)
    {
        winObject.transform.position = board.transform.position;
        winObject.SetActive(true);

        //--- More human-friendly format ---//
        // winDataText.text = $"You were warned {e.warningAmt} times and spend {e.warningTime} seconds in a warning state";

        //--- Simple statistics listing ---//
        winDataText.text = $"Warned: {e.warningAmt} times\nWarn Time: {e.warningTime}s";
    }

    void OnGameLost(GameLost e)
    {
        loseObject.transform.position = board.transform.position;
        loseObject.SetActive(true);
    }

    [SerializeField] string barrelDesignerScene = "BarrelDesigner";


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
