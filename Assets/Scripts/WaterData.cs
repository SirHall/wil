using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Excessives;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

public class WaterData : MonoBehaviour
{
    // This class allows us to find the exact height of the water at any point

    public static WaterData Instance { get; private set; }

    [SerializeField] Vector2 scale = Vector2.one * 100.0f;
    [SerializeField] Vector2 center = Vector2.zero;
    [SerializeField] float displacement = 0.7f;

    [SerializeField] Material waterMat;

    // The side length of the plane mesh we're using is 20m
    const float sideLength = 10;


    [SerializeField] bool debugMode = false;
    [SerializeField] [ShowIf("@debugMode")] MeshRenderer debugRenderer;

    [SerializeField] CustomRenderTexture heightRendTex;

    Texture2D tRead;

    void Awake()
    {
        transform.localScale = new Vector3(scale.x, 1.0f, scale.y);
        transform.position = center;
        tRead = new Texture2D(100, 100, DefaultFormat.HDR, TextureCreationFlags.None);
        // TODO: Set shader displacement from here
    }

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;

        GameplaySettingEvent.RegisterListener(OnGameplaySettingEvent);
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;

        GameplaySettingEvent.UnregisterListener(OnGameplaySettingEvent);
    }

    // The world position of the water plane's UV origin
    Vector2 UVOriginPos { get => (Vector2.one * (sideLength * 0.5f) * scale); }

    public Vector2 WorldPosToUV(Vector3 pos)
    {
        Vector2 uv = Vector2.one - ((-new Vector2(pos.x, pos.z) / (sideLength * scale)) + (Vector2.one * 0.5f));
        if (debugMode)
            debugRenderer.material.SetVector("UV", uv);
        return uv;
    }

    // public float EvalAtWorldPos(Vector3 pos) =>
    //     EvaluateOffsetAtUVPos(WorldPosToUV(pos), 20.0f, displacement);

    public float EvalAtWorldPos(Vector3 pos)
    {
        Vector2 uv = WorldPosToUV(pos);

        RenderTexture.active = heightRendTex;
        // float height = tRead.GetPixelBilinear(uv.x, uv.y).r;
        tRead.ReadPixels(new Rect(0, 0, 100, 100), 0, 0, false);
        RenderTexture.active = null;

        return (tRead as Texture2D).GetPixelBilinear(uv.x, uv.y).r;
        // return height;
    }
    void OnGameplaySettingEvent(GameplaySettingEvent e)
    {
        displacement = e.settings.bobbing ? displacement : 0.2f;
        print("Water Displacement: " + waterMat.GetFloat("_Displacement"));
        waterMat.SetFloat("_Displacement", displacement);
    }
}
