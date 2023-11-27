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

    private void Awake()
    {
        targetTransform = transform;
        originalPosition = targetTransform.localPosition;
    }

    public void StartShake()
    {
        StartCoroutine(ShakeCoroutine());
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
