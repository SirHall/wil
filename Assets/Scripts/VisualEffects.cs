using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class VisualEffects : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Post Processing Volume Gameobject")]
    private GameObject volume;

    [SerializeField]
    [Tooltip("Water Gameobject")]
    private GameObject water;

    [SerializeField]
    [Tooltip("The gameobject for visual effect images to be displayed over screen")]
    private GameObject visualEffects;

    [SerializeField]
    [Tooltip("Water Screen Effect Image")]
    private Image waterScreenEffect;

    private Volume postProcessingVolume;

    float test = 0;

    // Players head position
    Vector3 headPos = new Vector3();

    void OnEnable()
    {
        VisualControlEvent.RegisterListener(OnMoveControlEvent);
        WaterScreenEvent.RegisterListener(HeadWaterEffect);
    }

    void OnDisable()
    {
        VisualControlEvent.UnregisterListener(OnMoveControlEvent);
        WaterScreenEvent.UnregisterListener(HeadWaterEffect);
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
        if (visualEffects.GetComponentInChildren<Canvas>().worldCamera != Camera.main)
        {
            visualEffects.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            visualEffects.GetComponentInChildren<Canvas>().planeDistance = 0.15f;
        }

        LeaningEffect();
        UnderwaterEffect();
    }
    /// <summary>
    /// Water screen splash effect when players head gets too close to the barrel wave
    /// </summary>
    /// <param name="e"></param>
    private void HeadWaterEffect(WaterScreenEvent e)
    {
        waterScreenEffect.gameObject.SetActive(e.alphaValue > 0);

        float clampedValue = Mathf.Clamp(e.alphaValue, 0, 0.8f);

        Color waterColor = waterScreenEffect.color;
        waterColor.a = clampedValue;
        waterScreenEffect.color = waterColor;
    }

    /// <summary>
    /// Enables the leaning effects when the players head is detected leaning from the center of the board
    /// </summary>
    private void LeaningEffect()
    {
        if (postProcessingVolume.profile.TryGet<Vignette>(out var vignette))
        {
            if (!WaveScore.IsPlaying || WaveScore.IsWarmup)
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
        bool isUnderWater;
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
