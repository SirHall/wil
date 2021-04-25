using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "WaveAnim", menuName = "Barrel Wave Animation", order = 1)]
public class WaveAnim : ScriptableObject
{
    [Title("Barrel Wave Animation")]

    // As this class is purely a data-storage class,
    // we will be fine with public fields.

    [Tooltip("The z-position of the barrel relative to its starting position")]
    public AnimationCurve forwardPosition = new AnimationCurve();
    [Tooltip("Range 0 -> 1, 0 = barrel is of height 0, 1 = barrel is doing the full 360 degrees")]
    public AnimationCurve barrelArcAngle = new AnimationCurve();
    [Tooltip("The radius of the barrel circle")]
    public AnimationCurve barrelRadius = new AnimationCurve();
}
