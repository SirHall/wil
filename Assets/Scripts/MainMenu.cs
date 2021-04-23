using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] Button button;

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}
