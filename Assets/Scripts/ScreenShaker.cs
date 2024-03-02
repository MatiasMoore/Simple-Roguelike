using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ScreenShaker : MonoBehaviour
{
    public static ScreenShaker Instance;

    [SerializeField]
    private CinemachineImpulseListener _impulseListener;

    private CinemachineImpulseSource _impulseSource;
    private CinemachineImpulseDefinition _impulseDefinition;



    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _impulseDefinition = _impulseSource.m_ImpulseDefinition;
    }

    public void ShakeScreen(ScreenShakeProfile profile)
    {
        SetUpData(profile);
        _impulseSource.GenerateImpulse();
        
    }

    private void SetUpData(ScreenShakeProfile profile)
    {
        _impulseDefinition.m_ImpulseDuration = profile.sourceDuration;
        _impulseSource.m_DefaultVelocity = profile.sourceDeafaultVelocity * profile.VelocityScaler;
        _impulseDefinition.m_CustomImpulseShape = profile.sourceShape;

        _impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        _impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        _impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }

}
