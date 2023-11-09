using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ObjectShakeScript : MonoBehaviour
{
    public float shakeDuration = 0.2f;
    public float shakeStrength = 10f;

    private Vector3 originalPosition;
    private Transform targetTransform;
    private CinemachineVirtualCamera cine;
    public Transform player;

    private void Awake()
    {
        targetTransform = transform;
        originalPosition = targetTransform.localPosition;
        cine = GetComponent<CinemachineVirtualCamera>();
        if (player != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            player = cine.Follow;
        }
        else
        {
            return;
        }
    }

    public void StartShake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    public void CineShake()
    {
        cine.Follow = null;
        StartCoroutine(ShakeCoroutine());
    }

    public void ReAssignCam()
    {
        cine.Follow = player;
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;
            targetTransform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the position to the original after the shake is finished
        targetTransform.localPosition = originalPosition;
    }

}
