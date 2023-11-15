using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectExpandShrink : MonoBehaviour
{
    public float scaleSpeed = 2f; // Adjust the speed as needed
    public float maxScale = 1.5f;
    public float minScale = 0.5f;

    private bool isExpanding = true;

    void Update()
    {
        // Check if the sprite is expanding or shrinking
        if (isExpanding)
        {
            // Increase the scale over time
            transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;

            // Check if the max scale is reached
            if (transform.localScale.x >= maxScale)
            {
                isExpanding = false; // Switch to shrinking
            }
        }
        else
        {
            // Decrease the scale over time
            transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;

            // Check if the min scale is reached
            if (transform.localScale.x <= minScale)
            {
                isExpanding = true; // Switch to expanding
            }
        }
    }
}
