using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsToggle : MonoBehaviour
{
    [SerializeField] private GameObject terrain;
    [SerializeField] private GameObject coral;

    void OnEnable()
    {
        GameSettingsEvent.RegisterListener(OnGameplaySettingEvent);
    }

    void OnDisable()
    {
        GameSettingsEvent.UnregisterListener(OnGameplaySettingEvent);
    }

    void OnGameplaySettingEvent(GameSettingsEvent e)
    {
        terrain.SetActive(e.performanceSettings.isTerrain);
        coral.SetActive(e.performanceSettings.isCoral);
    }
}
