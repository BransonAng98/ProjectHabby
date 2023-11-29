using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeParticleInParent : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float delayFadeDuration = 2f;

    private ParticleSystem particles;
    private float initialAlpha;
    private float targetAlpha = 0f;

    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        if (particles == null)
        {
            Debug.LogError("No Particle System found in children. Make sure it's a child of the empty GameObject.");
            Destroy(gameObject);
            return;
        }

        var mainModule = particles.main;
        initialAlpha = mainModule.startColor.color.a;
        StartFading();
    }

    public void StartFading()
    {
        Invoke("DelayedFade", delayFadeDuration);
    }

    void DelayedFade()
    {
        StartCoroutine(FadeParticle());
    }

    private IEnumerator FadeParticle()
    {
        var mainModule = particles.main;

        float elapsedTime = 0f;
        Color startColor = mainModule.startColor.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < fadeDuration)
        {
            mainModule.startColor = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainModule.startColor = targetColor;

        // Destroy both the Particle System and the empty GameObject
        Destroy(particles.gameObject);
        Destroy(gameObject);
    }
}
