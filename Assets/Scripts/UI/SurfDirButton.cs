using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurfDirButton : MonoBehaviour
{
    [SerializeField] TextMeshPro buttonText;
    [SerializeField] GameObject dirArrow;
    [SerializeField] Transform waveTransform;

    [SerializeField] bool arrowRotateOnly = false;

    void OnEnable() => VRButtonEvent.RegisterListener(OnVRButtonEvent);
    void OnDisable() => VRButtonEvent.UnregisterListener(OnVRButtonEvent);

    void Start() => UpdateArrow();

    void OnVRButtonEvent(VRButtonEvent e)
    {
        if (e.button == VRButtons.SurfDir)
            BarrelSettings.Instance.SurfDir = ((BarrelSettings.Instance.SurfDir == RightLeft.Right) ? RightLeft.Left : RightLeft.Right);

        bool isRight = BarrelSettings.Instance.SurfDir == RightLeft.Right;

        buttonText.text = isRight ? "Right" : "Left";

        UpdateArrow();
    }

    void UpdateArrow()
    {
        bool isRight = BarrelSettings.Instance.SurfDir == RightLeft.Right;

        Vector3 waveRight = waveTransform.position + (waveTransform.right * BarrelSettings.Instance.Length);
        Vector3 waveLeft = waveTransform.position;

        dirArrow.transform.position = arrowRotateOnly ? waveLeft : (isRight ? waveLeft : waveRight); // Nested ternarys :)
        dirArrow.transform.rotation = Quaternion.LookRotation(isRight ? Vector3.right : Vector3.left);
    }
}
