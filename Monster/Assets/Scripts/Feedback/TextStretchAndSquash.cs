using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextStretchAndSquash : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    public float stretchAmount = 1.2f; // Adjust this value to control the stretch amount
    public float squashAmount = 0.8f; // Adjust this value to control the squash amount
    public float animationDuration = 0.5f; // Adjust this value to control the animation duration

    private bool isAnimating = false;

    void Start()
    {
        // Ensure the UI Text is not null
        if (uiText == null)
        {
            Debug.LogError("UI Text not assigned to the script!");
        }

        else
        {
            uiText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void StretchSquashAnimation()
    {
        isAnimating = true;

        // Stretch
        StartCoroutine(AnimateScale(new Vector3(stretchAmount, 1f, 1f), animationDuration / 2f, () =>
        {
            // Squash
            StartCoroutine(AnimateScale(new Vector3(squashAmount, 1f, 1f), animationDuration / 2f, () =>
            {
                // Animation complete
                isAnimating = false;
            }));
        }));
    }

    IEnumerator AnimateScale(Vector3 targetScale, float duration, System.Action onComplete = null)
    {
        Vector3 startScale = uiText.transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            uiText.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        uiText.transform.localScale = targetScale;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}
