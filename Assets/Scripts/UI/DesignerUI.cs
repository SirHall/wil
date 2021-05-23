using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DesignerUI : MonoBehaviour
{
    [SerializeField] Wave wave;

    [SerializeField] Slider barrelRadiusSlider;
    [SerializeField] Slider barrelArcSlider;
    [SerializeField] Slider barrelLengthSlider;
    [SerializeField] Slider barrelSquashSlider;

    [SerializeField] Button surfButton;

    #region Bookkeeping

    bool clickedSurf = false;

    #endregion

    void Start()
    {
        Time.timeScale = 1.0f;

        //  Firstly get the values from the settings, this allows us to use the
        // settings used from previous designer scene instances and allows for
        // apreviously run barrel to be slightly tweaked.
        barrelRadiusSlider.value = BarrelSettings.Instance.Radius;
        barrelArcSlider.value = BarrelSettings.Instance.Arc;
        barrelLengthSlider.value = BarrelSettings.Instance.Length;

        barrelRadiusSlider.onValueChanged.AddListener(UpdateBarrelRadius);
        barrelArcSlider.onValueChanged.AddListener(UpdateBarrelArc);
        barrelLengthSlider.onValueChanged.AddListener(UpdateBarrelLength);
        // barrelSquashSlider.onValueChanged.AddListener(v => wave.BarrelLength = v);

        UpdateBarrelRadius(barrelRadiusSlider.value);
        UpdateBarrelArc(barrelArcSlider.value);
        UpdateBarrelLength(barrelLengthSlider.value);
    }

    void UpdateBarrelRadius(float v) { wave.BarrelRadius = v; BarrelSettings.Instance.Radius = v; }
    void UpdateBarrelArc(float v) { wave.BarrelArc = v; BarrelSettings.Instance.Arc = v; }
    void UpdateBarrelLength(float v) { wave.BarrelLength = v; BarrelSettings.Instance.Length = v; }



    public void OnSurfButtonClicked()
    {
        if (!clickedSurf)
        {
            clickedSurf = true;
            StartCoroutine(LoadSim());
        }
    }

    IEnumerator LoadSim()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        load.allowSceneActivation = true;
        while (!load.isDone)
            yield return null;
        SceneManager.UnloadSceneAsync("BarrelDesigner");
    }

}
