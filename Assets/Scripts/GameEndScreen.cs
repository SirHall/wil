using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndScreen : MonoBehaviour
{
    public static GameEndScreen Instance { get; private set; }

    [SerializeField] [FoldoutGroup("Game Won")] GameObject winObject;
    [SerializeField] [FoldoutGroup("Game Won")] TextMeshProUGUI winDataText;

    [SerializeField] [FoldoutGroup("Game Lost")] GameObject loseObject;

    // [SerializeField] [FoldoutGroup("Buttons")] ;


    void OnEnable()
    {
        Instance = this;
        GameWon.RegisterListener(OnGameWon);
        GameLost.RegisterListener(OnGameLost);
    }

    void OnDisable()
    {
        Instance = null;
        GameWon.UnregisterListener(OnGameWon);
        GameLost.UnregisterListener(OnGameLost);
    }

    void OnGameWon(GameWon e)
    {
        winObject.SetActive(true);
        loseObject.SetActive(false);

        //--- More human-friendly format ---//
        // winDataText.text = $"You were warned {e.warningAmt} times and spend {e.warningTime} seconds in a warning state";

        //--- Simple statistics listing ---//
        winDataText.text = $"Warned: {e.warningAmt} times\nWarn Time: {e.warningTime}s";
    }

    void OnGameLost(GameLost e)
    {
        winObject.SetActive(false);
        loseObject.SetActive(true);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
    }

    public void OnRetryButton()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void OnBarrelDesignerButton()
    {
        SceneManager.LoadSceneAsync("BarrelDesigner", LoadSceneMode.Single);
    }
}
