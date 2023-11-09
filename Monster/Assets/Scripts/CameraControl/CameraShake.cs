using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public float shakeStrength;
    public float shakeFrequency;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera component not found on the GameObject.");
            return;
        }

        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin component not found on the CinemachineVirtualCamera.");
            return;
        }
    }

    public void ShakeCamera()
    {

        noise.m_AmplitudeGain = shakeStrength;
        noise.m_FrequencyGain = shakeFrequency;

    }

    public void StopShaking()
    {
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }
    }
}
