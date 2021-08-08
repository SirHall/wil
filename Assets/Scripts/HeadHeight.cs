using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHeight : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameObject that contains the VR camera")]
    GameObject vr_CameraGameObject;

    [SerializeField]
    [Tooltip("The GameObject that contains the Keyboard camera")]
    GameObject keyboard_CameraGameObject;

    [SerializeField]
    [Tooltip("The GameObject that contains the active camera")]
    GameObject activeCamera;
    // Update is called once per frame
    void Update()
    {
        // Set activeCamera to the camera active in hierarchy (Either keyboard or VR camera)
        if (vr_CameraGameObject.activeInHierarchy) activeCamera = vr_CameraGameObject;
        else if (keyboard_CameraGameObject.activeInHierarchy) activeCamera = keyboard_CameraGameObject;
        else activeCamera = keyboard_CameraGameObject; // Fallback

        CallGlobalEvents();
    }

    private void CallGlobalEvents()
    {
        using (var e = VisualControlEvent.Get())
        {
            e.dir.y = activeCamera.transform.localPosition.y;
        }
    }
}
