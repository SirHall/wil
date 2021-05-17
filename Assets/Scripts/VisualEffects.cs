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

    MovementInput input = new MovementInput();

    void OnEnable() {
        VisualControlEvent.RegisterListener(OnMoveControlEvent);
    }

    void OnDisable() {
        VisualControlEvent.UnregisterListener(OnMoveControlEvent);
    }

    // A controller has announced new data
    void OnMoveControlEvent(VisualControlEvent e) {
        input = e.input;
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
            float headPosDist = Mathf.Max(Mathf.Abs(input.dir.z), Mathf.Abs(input.dir.x));

            vignette.intensity.overrideState = true;
            vignette.intensity.value = headPosDist;
        }
    }
}
