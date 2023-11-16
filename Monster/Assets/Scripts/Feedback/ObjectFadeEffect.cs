using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFadeEffect : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Time it takes to fade in/out
    public float delayFadeDuration = 5f;
    private Renderer objectRenderer;
    private Color targetColor;
    private Color initialColor;
 

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        initialColor = objectRenderer.material.color;
        targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
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
        if(objectRenderer.material.color == targetColor)
        {
            Destroy(gameObject,1f);
        }
    }
}
