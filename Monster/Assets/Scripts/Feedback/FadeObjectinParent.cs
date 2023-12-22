using System.Collections;
using UnityEngine;

public class FadeObjectinParent : MonoBehaviour
{
    public float fadeDuration = 3f;
    public float delayFadeDuration = 2f;

    private SpriteRenderer objectRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private Color initialColor;

    void Start()
    {
        objectRenderer = GetComponentInChildren<SpriteRenderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer System found in children. Make sure it's a child of the empty GameObject.");
            Destroy(gameObject);
            return;
        }

        objectRenderer.GetPropertyBlock(materialPropertyBlock);
        initialColor = materialPropertyBlock.GetColor("_Color");
        
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
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        while (elapsedTime < fadeDuration)
        {
            Color currentColor = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
            materialPropertyBlock.SetColor("_Color", currentColor);
            objectRenderer.SetPropertyBlock(materialPropertyBlock);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        materialPropertyBlock.SetColor("_Color", targetColor);
        objectRenderer.SetPropertyBlock(materialPropertyBlock);

        DestroyFadedObj();
    }

    public void DestroyFadedObj()
    {
        Color finalColor = materialPropertyBlock.GetColor("_Color");
        if (finalColor.a == 0)
        {
            Destroy(objectRenderer.gameObject);
            Destroy(gameObject);
        }
    }
}