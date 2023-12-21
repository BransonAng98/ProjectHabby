using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvasImage : MonoBehaviour
{

    public float fadeDuration = 2f; // Duration of the fade-in effect in seconds
    public GameObject endScreen;
    public GameObject scoredisplay;



    public void StartFade()
    {
        StartCoroutine(FadeIn(GetComponent<Image>()));
    }

    IEnumerator FadeIn(Image image)
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;

        // Fade in
        while (elapsedTime < fadeDuration)
        {
            image.color = Color.Lerp(initialColor, new Color(initialColor.r, initialColor.g, initialColor.b, 0.8f), elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is exactly 1 at the end
        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0.8f);
        endScreen.SetActive(true);
        scoredisplay.SetActive(true);

    }
}
