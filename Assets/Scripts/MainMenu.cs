using System.Collections;
using System.Collections.Generic;
using Excessives;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{

    [SerializeField] float fadeTime = 5.0f;
    [SerializeField] Button button;
    [SerializeField] VideoPlayer video;
    // UI elements to move out of the way when loading the game
    [SerializeField] List<RectTransform> moveUITransforms = new List<RectTransform>();
    [SerializeField] AudioSource audioSource;
    [SerializeField] Camera menuCam;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] float titleFadeInTime = 3.0f;

    [SerializeField] TextMeshProUGUI teamLabel;
    [SerializeField] float teamLabelFadeinTime = 3.0f;

    #region Bookkeeping

    bool clickedLoad = false;

    #endregion

    void Awake()
    {
        StartCoroutine(Menu());
    }

    IEnumerator Menu()
    {
        title.alpha = 0.0f;
        teamLabel.alpha = 0.0f;
        yield return FadeInLabel(title, titleFadeInTime);
        yield return FadeInLabel(teamLabel, teamLabelFadeinTime);
    }

    IEnumerator FadeInLabel(TextMeshProUGUI label, float fadeInTime)
    {
        float clock = 0.0f;
        label.alpha = 0.0f;

        while (clock <= titleFadeInTime)
        {
            clock += Time.deltaTime;
            label.alpha = clock / fadeInTime;
            yield return null;
        }
    }

    public void OnPlayButtonClicked()
    {
        if (!clickedLoad)
        {
            clickedLoad = true;
            StartCoroutine(LoadGame());
        }
    }

    IEnumerator LoadGame()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("BarrelDesigner", LoadSceneMode.Additive);
        load.allowSceneActivation = true;
        while (!load.isDone)
            yield return null;

        Destroy(button);

        // float initCamFarPlane = menuCam.farClipPlane;
        menuCam.nearClipPlane = 0.001f;
        menuCam.farClipPlane = 0.01f;

        //Now transition to the other game
        for (float start = Time.time; Time.time <= start + fadeTime;)
        {
            // Get a value in range 0 -> 1, where 0 is the
            // start time, 1 is the end time
            float t = MathE.UnLerp(start, start + fadeTime, Time.time);
            video.targetCameraAlpha = 1.0f - t;
            audioSource.volume = 1.0f - t;
            title.alpha = 1.0f - t;
            teamLabel.alpha = 1.0f - t;
            yield return null;
        }

        Scene barrelScene = SceneManager.GetSceneByName("BarrelDesigner");
        SceneManager.SetActiveScene(barrelScene);
        SceneManager.UnloadSceneAsync("Main Menu");
    }
}
