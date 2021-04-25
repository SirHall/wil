using System.Collections;
using System.Collections.Generic;
using Excessives;
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

    bool clickedLoad = false;

    public void OnPlayButtonClicked()
    {
        if (!clickedLoad)
        {
            StartCoroutine(LoadGame());
            clickedLoad = true;
        }
    }

    IEnumerator LoadGame()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
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
            // menuCam.farClipPlane = Mathf.Lerp(initCamFarPlane, 0.0f, t);
            yield return null;
        }

        SceneManager.UnloadSceneAsync("Main Menu");
    }
}
