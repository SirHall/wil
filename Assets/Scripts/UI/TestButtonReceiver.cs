using System.Collections;
using System.Collections.Generic;
using Excessives.LinqE;
using UnityEngine;

public class TestButtonReceiver : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] List<AudioClip> clips = new List<AudioClip>();

    void OnEnable() => VRButtonEvent.RegisterListener(OnTestVRButtonEvent);
    void OnDisable() => VRButtonEvent.UnregisterListener(OnTestVRButtonEvent);

    void OnTestVRButtonEvent(VRButtonEvent e) => audioSource.PlayOneShot(clips.Pick());
}
