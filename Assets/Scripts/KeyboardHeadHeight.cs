using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHeadHeight : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameObject that contains the Keyboard camera")]
    GameObject keyboard_CameraGameObject;

    // Update is called once per frame
    void Update()
    {
        CallGlobalEvents();
    }

    private void CallGlobalEvents()
    {
        using (var e = VisualControlEvent.Get())
        {
            e.dir.y = keyboard_CameraGameObject.transform.position.y;
        }

        using (var e = SoundControlEvent.Get())
        {
            e.headInput.dir.y = keyboard_CameraGameObject.transform.position.y;
        }
    }
}
