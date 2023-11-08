using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrike : MonoBehaviour
{
    public int numberOfPlanes;
    public GameObject enemyPlane;

    private Animator anim;
    public Transform player;
    public Transform[] spawnPoints;
    bool isActivating;
    bool isBombing;

    public float blinkSpeed = 0.1f; // Speed of the blinking effect
    public float minAlpha = 0.0f;   // Minimum alpha value (fully transparent)
    public float maxAlpha = 1.0f;   // Maximum alpha value (fully opaque)
    public float lifeDuration = 1.5f; // Time the zone will exist before being destroyed
    private float currentLife;

    public GameObject warningZone;
    private Color originalColor;
    private float currentAlpha;
    private float blinkDirection = 1.0f; // Used to control the blinking direction


    private void Start()
    {
        anim = GameObject.Find("MilitaryAbilityWarning").GetComponent<Animator>();

        // Check if we have at least one spawn point
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        if (warningZone == null)
        {
            Debug.LogError("SpriteRenderer component not found.");
            return;
        }

        originalColor = warningZone.GetComponent<SpriteRenderer>().color;
        currentAlpha = originalColor.a;


    }

    private void Update()
    {
        currentLife += Time.deltaTime;

        if (isBombing == true & currentLife <= lifeDuration)
        {
            BlinkingEffect();
        }
        else // Turn off after the specified duration
        {
            warningZone.SetActive(false);
            currentLife = 0;
            isBombing = false;

        }
    }

    void BlinkingEffect()
    {
        if (isBombing == true)
        {
            warningZone.SetActive(true);
            // Update the alpha value to create a blinking effect
            currentAlpha += blinkDirection * blinkSpeed * Time.deltaTime;
            currentAlpha = Mathf.Clamp(currentAlpha, minAlpha, maxAlpha);

            // Apply the new color with the updated alpha value
            Color newColor = warningZone.GetComponent<SpriteRenderer>().color;
            newColor.a = currentAlpha;
            warningZone.GetComponent<SpriteRenderer>().color = newColor;

            // Change blinking direction at the alpha limits
            if (currentAlpha <= minAlpha || currentAlpha >= maxAlpha)
            {
                blinkDirection *= -1.0f;
            }

        }

    }

    public void ActivateAirStrike()
    {
        if (enemyPlane != null)
        {

            Invoke("DeactiveBanner", 3f);

            anim.SetBool("Close", true);

            Invoke("RandomizeAndSpawn", 6f);

            Invoke("ResetActivation", 15f);

        }

    }

    void DeactiveBanner()
    {
        anim.SetBool("Close", false);
    }

    public void RandomizeAndSpawn()
    {
        // Randomly choose a spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the GameObject at the desired position
        isBombing = true;
        warningZone.transform.position = player.transform.position;

        // Set spawn points to player y pos
        spawnPoints[0].position = new Vector3(spawnPoints[0].position.x, player.position.y, spawnPoints[0].position.z);
        spawnPoints[1].position = new Vector3(spawnPoints[1].position.x, player.position.y, spawnPoints[1].position.z);

        SpawnObject(randomSpawnPoint);

    }

    public void SpawnObject(Transform spawnPoint)
    {

        // Define the stagger amount and initial offset
        float staggerAmountX = 2.0f;  // Adjust this based on desired spacing along x-axis
        float staggerAmountY = 2.0f;  // Adjust this based on desired spacing along y-axis
        float initialOffsetX = -staggerAmountX * 1.5f;  // Adjust initial offsets based on the formation
        float initialOffsetY = -staggerAmountY;  // Adjust initial offsets based on the formation

        for (int i = 0; i < numberOfPlanes; i++)
        {
            // Calculate the staggered position for each fighter plane
            float xOffset = initialOffsetX + i * staggerAmountX;
            float yOffset = initialOffsetY + i * staggerAmountY;
            Vector3 staggeredPosition = new Vector3(spawnPoint.position.x + xOffset, spawnPoint.position.y + yOffset, 0f);

            // Instantiate the fighter jet at the staggered position
            GameObject fighterJet = Instantiate(enemyPlane, staggeredPosition, spawnPoint.rotation);
        }
    }
}
