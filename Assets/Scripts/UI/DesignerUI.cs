using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DesignerUI : MonoBehaviour
{

    [Tooltip("The length of the barrel when surfing")]
    [SerializeField] [TabGroup("Settings")] float barrelLength = 50.0f;
    [Tooltip("How much to change the barrel length by within the editor")]
    [SerializeField] [TabGroup("Settings")] float barrelLengthDelta = 5.0f;
    [Tooltip("The minimum and maximum length of the barrel")]
    [SerializeField] [TabGroup("Settings")] [MinMaxSlider(0.0f, 500.0f, true)] Vector2 barrelLengthLimits = new Vector2(5.0f, 150.0f);

    [Space]
    [Tooltip("The radius of the barrel reached during the surfing intro animation")]
    [SerializeField] [TabGroup("Settings")] float barrelRadius = 4.0f;
    [Tooltip("How much to change the barrel radius by within the editor")]
    [SerializeField] [TabGroup("Settings")] float barrelRadiusDelta = 0.5f;
    [Tooltip("The minimum and maximum radius of the barrel")]
    [SerializeField] [TabGroup("Settings")] [MinMaxSlider(0.0f, 50.0f, true)] Vector2 barrelRadiusLimits = new Vector2(1.0f, 8.0f);


    [SerializeField] [TabGroup("References")] Wave wave;
    [SerializeField] [TabGroup("References")] string surfScene = "Game";
    [SerializeField] [TabGroup("References")] string mainMenuScene = "Main Menu";

    #region Bookkeeping

    bool clickedSurf = false;

    #endregion



    void Start()
    {
        Time.timeScale = 1.0f;

        UpdateBarrelLength(barrelLength);
        UpdateBarrelRadius(barrelRadius);
        UpdateBarrelArc(0.8f);
    }

    void UpdateBarrelLength(float v)
    {
        barrelLength = v.Clamp(barrelLengthLimits);
        wave.BarrelLength = barrelLength;
        BarrelSettings.Instance.Length = barrelLength;
    }

    void UpdateBarrelRadius(float v)
    {
        barrelRadius = v.Clamp(barrelRadiusLimits);
        wave.BarrelRadius = barrelRadius;
        BarrelSettings.Instance.Radius = barrelRadius;
    }

    void UpdateBarrelArc(float v) { wave.BarrelArc = v; BarrelSettings.Instance.Arc = v; }


    void OnEnable() => VRButtonEvent.RegisterListener(OnVRButtonEvent);
    void OnDisable() => VRButtonEvent.UnregisterListener(OnVRButtonEvent);


    void OnVRButtonEvent(VRButtonEvent e)
    {
        switch (e.button)
        {
            case VRButtons.Surf: SceneManager.LoadScene(surfScene, LoadSceneMode.Single); break;
            case VRButtons.Exit: SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single); break;

            case VRButtons.Designer_IncreaseRadius: UpdateBarrelRadius(barrelRadius + barrelRadiusDelta); break;
            case VRButtons.Designer_DecreaseRadius: UpdateBarrelRadius(barrelRadius - barrelRadiusDelta); break;

            case VRButtons.Designer_Longer: UpdateBarrelLength(barrelLength + barrelLengthDelta); break;
            case VRButtons.Designer_Shorter: UpdateBarrelLength(barrelLength - barrelLengthDelta); break;

            default:
                break; // Ignore all other buttons
        }
    }

}
