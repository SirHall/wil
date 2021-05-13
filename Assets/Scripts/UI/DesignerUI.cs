using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesignerUI : MonoBehaviour
{
    [SerializeField] Wave wave;

    [SerializeField] Slider barrelRadiusSlider;
    [SerializeField] Slider barrelArcSlider;
    [SerializeField] Slider barrelLengthSlider;

    void Start()
    {
        barrelRadiusSlider.onValueChanged.AddListener(v => wave.BarrelRadius = v);
        barrelArcSlider.onValueChanged.AddListener(v => wave.BarrelArc = v);
        barrelLengthSlider.onValueChanged.AddListener(v => wave.BarrelLength = v);
    }

}
