using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] TextMeshPro button_bobbingText;
    [SerializeField] TextMeshPro button_introText;
    [SerializeField] TextMeshPro panel_warmupText;

    private bool isBobbing = true;

    private float warmupTime = 5;
    private float warmupTimeDelta = 5;
    private float warmupTimeMax = 60;
    private float warmupTimeMin = 0;

    private bool isIntro = true;

    void OnEnable() => VRButtonEvent.RegisterListener(OnVRButtonEvent);
    void OnDisable() => VRButtonEvent.UnregisterListener(OnVRButtonEvent);

    void Start()
    {
        UpdateStats();
    }

    void UpdateStats()
    {
        isBobbing = GameplaySettings.Instance.Bobbing;
        warmupTime = GameplaySettings.Instance.WarmupTime;
        isIntro = GameplaySettings.Instance.IntroStart;

        button_bobbingText.text = isBobbing ? "On" : "Off";
        panel_warmupText.text = warmupTime.ToString() + " Seconds";
        button_introText.text = isIntro ? "On" : "Off";
    }

    void UpdateBobbing()
    {
        isBobbing = GameplaySettings.Instance.Bobbing ? false : true;
        GameplaySettings.Instance.Bobbing = isBobbing;
        button_bobbingText.text = isBobbing ? "On" : "Off";
    }

    void UpdateWarmup(float time)
    {
        warmupTime = Mathf.Clamp(time, warmupTimeMin, warmupTimeMax);
        GameplaySettings.Instance.WarmupTime = warmupTime;
        panel_warmupText.text = warmupTime.ToString() + " Seconds";
    }

    void UpdateIntro()
    {
        isIntro = GameplaySettings.Instance.IntroStart ? false : true;
        GameplaySettings.Instance.IntroStart = isIntro;
        button_introText.text = isIntro ? "On" : "Off";
    }

    void OnVRButtonEvent(VRButtonEvent e)
    {
        switch (e.button)
        {
            case VRButtons.None:
                Debug.LogError("No event should be fired for a VRButton of type 'None'");
                break;

            case VRButtons.Options_Bobbing: UpdateBobbing(); break;
            case VRButtons.Options_IncreaseWarmup: UpdateWarmup(warmupTime + warmupTimeDelta); break;
            case VRButtons.Options_DecreaseWarmup: UpdateWarmup(warmupTime - warmupTimeDelta); break;
            case VRButtons.Options_IntroStart: UpdateIntro(); break;

            default:
                break; // Ignore all other buttons
        }
    }
}
