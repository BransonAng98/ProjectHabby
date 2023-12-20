using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fadeTextScript : MonoBehaviour
{
    public float fadeDuration = 2f; // Duration of the fade-out effect in seconds
    public float delayBeforeFadeOut = 2f; // Delay before starting the fade-out

    void Start()
    {
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Start the fade-out coroutine
        StartCoroutine(FadeOut(canvasGroup));
    }

    IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;

        // Wait for a delay before starting the fade-out
        yield return new WaitForSeconds(delayBeforeFadeOut);

        // Fade out
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is exactly 0 at the end
        canvasGroup.alpha = 0f;

        // Destroy the text object after fading out
        Destroy(gameObject);
    }
}
