using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    public Camera mainCamera;
    public Transform playerPos;
    public float minX = -5f; // Adjust these values based on your camera size and desired range
    public float maxX = 5f;
    public float minY = -5f;
    public float maxY = 5f;

    public GameObject artilleryPrefab;
    public Transform artilleryPos;
    public float moveSpeed;
    public GameObject CircleIndicatorPrefab;
    public GameObject explosionVFX;
    public GameObject impactCrater;
    private GameObject storedData;
    private Animator anim;
    private bool hasArtillerySpawned = false;

    private void Start()
    {
        CircleIndicatorPrefab.GetComponent<Collider2D>();
        anim = GameObject.Find("MilitaryAbilityWarning").GetComponent<Animator>();
    }
    private void ReactivateArtilleryScriptAfterDelay(float delay)
    {
        StartCoroutine(ReactivateArtilleryCoroutine(delay));
    }

    private IEnumerator ReactivateArtilleryCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the flag to allow artillery to spawn again
        hasArtillerySpawned = false;
    }

    public void ActivateArtillery()
    {
        if (!hasArtillerySpawned)
        {
            StartCoroutine(SpawnArtilleryWithDelay());

            Invoke("DeactiveBanner", 3f);

            anim.SetBool("Close", true);

            StartCoroutine(DelayedSpawnArti(3f));

            Invoke("ResetActivation", 15f);

            // Set the flag to true to indicate that artillery has been spawned
            hasArtillerySpawned = true;
        }
    }

    public IEnumerator SpawnArtilleryWithDelay()
    {

        GameObject circleIndicator = Instantiate(CircleIndicatorPrefab, playerPos.position, Quaternion.identity);

        // Wait for 0.3 seconds
        yield return new WaitForSeconds(0.3f);

        // Spawn the artillery after the delay
        SpawnArti();


    }
    public void SpawnArti()
    {
        // Define the number of game objects to instantiate
        int numberOfGameObjects = 5; // Adjust the number as needed

        // Define a radius for the circle of game objects
        float circleRadius = 2f; // Adjust the radius as needed

        for (int i = 0; i < numberOfGameObjects; i++)
        {
            // Calculate a random angle
            float randomAngle = Random.Range(0f, 360f);

            // Convert the angle to radians
            float angleInRadians = Mathf.Deg2Rad * randomAngle;

            // Calculate the position based on the angle and radius
            float posX = Mathf.Cos(angleInRadians) * circleRadius;
            float posY = Mathf.Sin(angleInRadians) * circleRadius;

            // Instantiate the game object at the calculated position relative to artilleryPos
            Vector3 spawnPosition = artilleryPos.position + new Vector3(posX, posY, 0);
            InstantiateYourGameObjectHere(spawnPosition); // Replace with the actual instantiation code
        }
    }

    private void InstantiateYourGameObjectHere(Vector3 position)
    {
        // Replace the following line with the actual instantiation code for your 2D game object
        Instantiate(artilleryPrefab, position, Quaternion.identity);
    }

    private IEnumerator DelayedSpawnArti(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        SpawnArti();
    }


}



