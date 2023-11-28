using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectinParent : MonoBehaviour
{
    public float fadeDuration = 3f;
    public float delayFadeDuration = 2f;

    private Renderer objectRenderer;
    private Color targetColor;
    private Color initialColor;

    void Start()
    {
        objectRenderer = GetComponentInChildren<SpriteRenderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer System found in children. Make sure it's a child of the empty GameObject.");
            Destroy(gameObject);
            return;
        }

        initialColor = objectRenderer.material.color;
        targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
       // StartFading();
    }

    public void StartFading()
    {
        Invoke("DelayedFade", delayFadeDuration);
    }

    void DelayedFade()
    {
        StartCoroutine(FadeObject());
    }

    private IEnumerator FadeObject()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            objectRenderer.material.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectRenderer.material.color = targetColor;
        DestroyFadedObj();
    }

    public void DestroyFadedObj()
    {
        if (objectRenderer.material.color == targetColor)
        {
            Destroy(objectRenderer.gameObject);
            Destroy(gameObject);
        }
    }
}
