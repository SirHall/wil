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
    [SerializeField] string barrelDesignerScene = "BarrelDesigner";

    void OnEnable() => VRButtonEvent.RegisterListener(OnVRButtonEvent);
    void OnDisable() => VRButtonEvent.UnregisterListener(OnVRButtonEvent);


    void OnVRButtonEvent(VRButtonEvent e)
    {
        switch (e.button)
        {
            case VRButtons.None:
                Debug.LogError("No event should be fired for a VRButton of type 'None'");
                break;
            case VRButtons.MainMenu_Start:
                // Move to the barrel designer
                SceneManager.LoadScene(barrelDesignerScene, LoadSceneMode.Single);
                break;
            case VRButtons.Exit:
                Utils.Quit(); // Close the game
                break;
            default:
                break; // Ignore all other buttons
        }
    }
}
