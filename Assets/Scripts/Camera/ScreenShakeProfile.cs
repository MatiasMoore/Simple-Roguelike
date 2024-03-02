using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeProfile", menuName = "ScriptableObjects/ScreenShakeProfile")]
public class ScreenShakeProfile : ScriptableObject
{
    [Header("Impulse source settings")]
    public float sourceDuration = 0.2f;
    public float impactForce = 1.0f;
    public Vector3 sourceDeafaultVelocity = new Vector3(0f, -1f, 0f);
    public float VelocityScaler;
    public AnimationCurve sourceShape;  

    [Header("Impulse listener settings")]
    public float listenerAmplitude = 0.2f;
    public float listenerFrequency = 1.0f;
    public float listenerDuration = 0.2f;
    
}
