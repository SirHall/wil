using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextFromSlider : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Slider slider;

    void OnEnable() => slider.onValueChanged.AddListener(OnSliderUpdate);
    void OnDisable() => slider.onValueChanged.AddListener(OnSliderUpdate);

    void Start() => OnSliderUpdate(slider.value);

    public void OnSliderUpdate(float val) => text.text = $"{val}";
}
