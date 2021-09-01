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

    [SerializeField]
    [Tooltip("Water Gameobject")]
    private GameObject water;

    private Volume postProcessingVolume;

    // Players head position
    Vector3 headPos = new Vector3();

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
        headPos = e.dir;
    }

    // Start is called before the first frame update
    void Start()
    {
        postProcessingVolume = volume.GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        LeaningEffect();
        UnderwaterEffect();
    }
    /// <summary>
    /// Enables the leaning effects when the players head is detected leaning from the center of the board
    /// </summary>
    private void LeaningEffect()
    {
        if (postProcessingVolume.profile.TryGet<Vignette>(out var vignette))
        {
            if (!WaveScore.IsPlaying)
            {
                vignette.intensity.value = 0;
                return;
            }

            float headPosDist = Mathf.Max(Mathf.Abs(headPos.z), Mathf.Abs(headPos.x));

            vignette.intensity.overrideState = true;
            vignette.intensity.value = headPosDist + 0.1f;
        }
    }
    /// <summary>
    /// Enables the underwater effects when the player is below the water level
    /// </summary>
    private void UnderwaterEffect()
    {
        bool isUnderWater = false;
        // Check if players position is below the water
        if (headPos.y <= 0)
        {
            water.transform.rotation = new Quaternion(180, 0, 0, 0);
            isUnderWater = true;
        }
        else
        {
            water.transform.rotation = new Quaternion(0, 0, 0, 0);
            isUnderWater = false;
        }

        //Enable or disable all existing effects on post processing game object which create an underwater effect.
        if (postProcessingVolume.profile.TryGet<Vignette>(out var vignette)) vignette.active = !isUnderWater;
        if (postProcessingVolume.profile.TryGet<DepthOfField>(out var dof)) dof.active = isUnderWater;
        if (postProcessingVolume.profile.TryGet<ColorAdjustments>(out var colorAdjust)) colorAdjust.active = isUnderWater;
        if (postProcessingVolume.profile.TryGet<ShadowsMidtonesHighlights>(out var shadows)) shadows.active = isUnderWater;
    }
}
