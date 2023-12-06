using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    public Camera mainCamera;
    public Transform playerPos;
    public int numberOfLocations = 10; // Change the number of locations here
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

    [SerializeField]
    private List<Vector3> objectPositions = new List<Vector3>();

    private void Start()
    {
        CircleIndicatorPrefab.GetComponent<Collider2D>();
        anim = GameObject.Find("MilitaryAbilityWarning").GetComponent<Animator>();
    }


    public void ActivateArtillery()
    {

        StartCoroutine(SpawnArtilleryWithDelay());

        Invoke("DeactiveBanner", 3f);

        anim.SetBool("Close", true);

        Invoke("ResetActivation", 15f);
    }

    public IEnumerator SpawnArtilleryWithDelay()
    {
        // Calculate local boundaries for random artillery positions
        Vector3 localMinBoundary = artilleryPos.InverseTransformPoint(playerPos.transform.position + new Vector3(minX, minY, 0));
        Vector3 localMaxBoundary = artilleryPos.InverseTransformPoint(playerPos.transform.position + new Vector3(maxX, maxY, 0));

        List<Vector3> circleIndicatorPositions = new List<Vector3>();
        List<Vector3> usedPositions = new List<Vector3>(); // Keep track of used positions

        for (int i = 0; i < numberOfLocations; i++)
        {
            Vector3 randomPosition = GetRandomPosition(localMinBoundary, localMaxBoundary, usedPositions);
            circleIndicatorPositions.Add(randomPosition);

            GameObject circleIndicator = Instantiate(CircleIndicatorPrefab, randomPosition, Quaternion.identity);
            storedData = circleIndicator;
            // Wait for 0.3 seconds before spawning the next circle indicator
            yield return new WaitForSeconds(0.3f);
        }
        // Wait for 3 seconds before spawning the artillery
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < numberOfLocations; i++)
        {
            Vector3 randomPosition = circleIndicatorPositions[i];
            GameObject artillery = Instantiate(artilleryPrefab, artilleryPos.position, Quaternion.identity);
            objectPositions.Add(randomPosition);


            artillery.GetComponent<ArtilleryBullet>().GetData(storedData);

            Vector3 moveDirection = (randomPosition - artillery.transform.position).normalized;
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x);
            float rotationAngle = Mathf.Rad2Deg * angle - 90f;
            artillery.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            StartCoroutine(MoveToPosition(artillery, artillery.transform, randomPosition));
            yield return new WaitForSeconds(0.3f);
        }
    }

    private Vector3 GetRandomPosition(Vector3 minBoundary, Vector3 maxBoundary, List<Vector3> usedPositions)
    {
        Vector3 randomPosition;

        do
        {
            float localX = Random.Range(minBoundary.x, maxBoundary.x);
            float localY = Random.Range(minBoundary.y, maxBoundary.y);
            randomPosition = artilleryPos.TransformPoint(new Vector3(localX, localY, 0));
        }
        while (IsTooClose(randomPosition, usedPositions));

        usedPositions.Add(randomPosition);
        return randomPosition;
    }

    private bool IsTooClose(Vector3 position, List<Vector3> positions, float minDistance = 1f)
    {
        foreach (Vector3 existingPosition in positions)
        {
            if (Vector3.Distance(position, existingPosition) < minDistance)
            {
                return true; // Too close to an existing position
            }
        }
        return false; // Not too close to any existing position
    }

    private IEnumerator MoveToPosition(GameObject artilleryBullet, Transform transform, Vector3 targetPosition)
    {
        if (artilleryBullet != null && transform != null)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
                if (Vector3.Distance(transform.position, targetPosition) < 1f)
                {
                    ArtiDamageColliderScript artillerycollider = artilleryBullet.GetComponentInChildren<ArtiDamageColliderScript>();
                    Debug.Log("OpenCollider");
                    artillerycollider.turnonCollider();

                    // Do something when the artillery bullet is 0.1f away from its position
                    // For example, play a sound effect or trigger a visual effect
                    Debug.Log("Artillery bullet is very close to the target position!");
                }
            }



            if (artilleryBullet != null)
            {
                ArtilleryBullet artilleryBulletComponent = artilleryBullet.GetComponent<ArtilleryBullet>();
                if (artilleryBulletComponent != null)
                {
                    artilleryBulletComponent.BlowUp();
                    //artilleryBulletComponent.PlayExplodeSFX();
                }

                // Destroy the artillery prefab instance
               Destroy(artilleryBullet);
                Vector2 spawnPos = new Vector2(targetPosition.x, targetPosition.y + 1.5f);

                // Create and play the explosion VFX
                GameObject explosion = Instantiate(explosionVFX, spawnPos, Quaternion.identity);

                // Wait for the VFX to finish playing
                yield return new WaitForSeconds(0.1f);

                GameObject impact = Instantiate(impactCrater, spawnPos, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("ArtilleryBullet or Transform is null in MoveToPosition");
            yield return null;
        }
    }

}



