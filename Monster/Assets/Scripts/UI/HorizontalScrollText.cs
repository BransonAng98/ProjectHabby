using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HorizontalScrollText : MonoBehaviour
{
    public TextMeshProUGUI textToMove;
    public float moveSpeed;
    public float moveDistance;
    public List<string> messages = new List<string>();
    private float screenWidth;
    private float resetX;


    void Start()
    {
        RandomizeMessage();
        if (textToMove == null)
        {
            Debug.LogError("TextMeshProUGUI not assigned to the script!");
            return;
        }

        // Get the screen width in world units
        screenWidth = Screen.width / 100.0f;

        // Calculate the reset position to the right of the screen
        resetX = screenWidth + textToMove.rectTransform.rect.width;

        // Start the horizontal movement coroutine
        StartCoroutine(MoveText());
    }

    void RandomizeMessage()
    {
        int random = Random.Range(0, 4 + 1);
        textToMove.text = messages[random];
    }

    IEnumerator MoveText()
    {
        while (true)
        {
            // Move the text to the left
            textToMove.rectTransform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // Check if the text has moved completely off the screen
            if (textToMove.rectTransform.anchoredPosition.x < -resetX)
            {
                // Reset the text position to the right of the screen
                RandomizeMessage();
                textToMove.rectTransform.anchoredPosition = new Vector2(resetX, textToMove.rectTransform.anchoredPosition.y);
            }

            yield return null;
        }
    }
}
