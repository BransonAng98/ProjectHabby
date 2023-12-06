using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularIndicator : MonoBehaviour
{
    public float flashDuration = 0.5f; // Time in seconds for each flash
    public Vector3 maxScale = new Vector3(2.0f, 2.0f, 2.0f);
    public Vector3 minScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Color startColor = Color.white; // Initial color of the object
    public Color endColor = new Color(1.0f, 1.0f, 1.0f, 0.0f); // Color with alpha set to 0 for fully transparent

    private bool scalingUp = true;
    public bool isInRange;

    private void Start()
    {
        StartCoroutine(FlashScale());
        StartCoroutine(DestroyAfterDelay(10f)); // Destroy the object after 5 seconds
    }

    private IEnumerator FlashScale()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;

        while (true) // Repeat indefinitely
        {
            float elapsedTime = 0f;
            Vector3 startScale = scalingUp ? minScale : maxScale;
            Vector3 endScale = scalingUp ? maxScale : minScale;
            Color startAlphaColor = scalingUp ? startColor : endColor;
            Color endAlphaColor = scalingUp ? endColor : startColor;

            while (elapsedTime < flashDuration)
            {
                Vector3 scale = Vector3.Lerp(startScale, endScale, elapsedTime / flashDuration);
                transform.localScale = scale;

                Color alphaColor = Color.Lerp(startAlphaColor, endAlphaColor, elapsedTime / flashDuration);
                material.color = alphaColor;

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Toggle scaling direction for the next iteration
            scalingUp = !scalingUp;
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}