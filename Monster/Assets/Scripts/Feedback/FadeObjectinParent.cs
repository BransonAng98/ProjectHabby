using System.Collections;
using UnityEngine;

public class FadeObjectinParent : MonoBehaviour
{
    public float fadeDuration;
    public float delayFadeDuration;

    private SpriteRenderer objectRenderer;
    private Color initialColor;
    private Color targetColor;

    void Start()
    {
        objectRenderer = GetComponentInChildren<SpriteRenderer>();
        initialColor = objectRenderer.material.color;
        targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer System found in children. Make sure it's a child of the empty GameObject.");
            Destroy(gameObject);
            return;
        }

    }

    public void StartFading()
    {
        Invoke(nameof(DelayedFade), delayFadeDuration);
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
    }

    
}