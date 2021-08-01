using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector Instance { get; private set; }

    [SerializeField] [TabGroup("Levels")] string mainMenu = "Main Menu";
    [SerializeField] [TabGroup("Levels")] string barrelDesigner = "BarrelDesigner";

    void Awake() => DontDestroyOnLoad(gameObject);

    void OnEnable()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        VRButtonEvent.RegisterListener(OnVRButtonEvent);
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
            VRButtonEvent.UnregisterListener(OnVRButtonEvent);
        }
    }

    void OnVRButtonEvent(VRButtonEvent e)
    {
        // switch (e.button)
        // {
        //     case VRButtons.None:
        //         Debug.LogError("No event should be fired for a VRButton of type 'None'");
        //         break;
        //     case VRButtons.MainMenu_Start:
        //         // Move to the barrel designer
        //         SceneManager.LoadScene(barrelDesigner, LoadSceneMode.Single);
        //         break;

        //     case VRButtons.Exit:
        //         Utils.Quit(); // Close the game
        //         break;

        //     case VRButtons.Designer:
        //         SceneManager.LoadScene(barrelDesigner, LoadSceneMode.Single);
        //         break;

        //     default:
        //         break; // Ignore all other buttons
        // }
    }
}
