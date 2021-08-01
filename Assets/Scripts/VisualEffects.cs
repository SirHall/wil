using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisualEffects : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Post Processing Volume Gameobject")]
    private GameObject volume;

    private Volume postProcessingVolume;

    Vector3 dir = new Vector3();

    void OnEnable()
    {
        VisualControlEvent.RegisterListener(OnMoveControlEvent);
    }

    void OnDisable()
    {
        VisualControlEvent.UnregisterListener(OnMoveControlEvent);
    }

    // A controller has announced new data
    void OnMoveControlEvent(VisualControlEvent e)
    {
        dir = e.dir;
    }

    // Start is called before the first frame update
    void Start()
    {
        postProcessingVolume = volume.GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (postProcessingVolume.profile.TryGet<Vignette>(out var vignette))
        {
            float headPosDist = Mathf.Max(Mathf.Abs(dir.z), Mathf.Abs(dir.x));

            vignette.intensity.overrideState = true;
            vignette.intensity.value = headPosDist + 0.1f;
        }
    }
}
