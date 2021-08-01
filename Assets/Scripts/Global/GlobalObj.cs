using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObj : MonoBehaviour
{
    public static GlobalObj Instance { get; private set; }

    void Awake() => DontDestroyOnLoad(gameObject);

    void OnEnable()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    [RuntimeInitializeOnLoadMethod]
    static void Create() => Instantiate(Resources.Load("Global"));
}
