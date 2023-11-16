using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleFadeEffect : MonoBehaviour
{
    public float fadeDuration = 2.0f;
    public float delayFadeDuration = 5f;

    private ParticleSystem particles;
    private float initialAlpha;
    private float targetAlpha = 0f;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
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
        Destroy(gameObject, 2f);
    }
}
