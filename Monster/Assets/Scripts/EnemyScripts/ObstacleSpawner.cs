using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnableObstacles;
    [SerializeField] private int minNumberOfObjectsToSpawn;
    [SerializeField] private int maxNumberOfObjectsToSpawn;
    [SerializeField] private float minDistanceBetweenObstacles;
    [SerializeField] private GameObject obstacleParent;
    private const int MaxAttempts = 100;

    private List<Vector2> spawnedPositions = new List<Vector2>();

    private void Start()
    {
        FindObstacleParent();

        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        foreach (BoxCollider2D boxCollider in colliders)
        {
            SpawnObstacles(boxCollider.bounds);
        }
    }

    private void FindObstacleParent()
    {
        if (obstacleParent == null)
        {
            obstacleParent = GameObject.Find("---Obstacles---");
        }
    }

    private void SpawnObstacles(Bounds colliderBounds)
    {
        for (int i = 0; i < Random.Range(minNumberOfObjectsToSpawn, maxNumberOfObjectsToSpawn + 1); i++)
        {
            if (Random.value > 0.25f) // Adjust probability as needed
            {
                continue;
            }

            Vector2 randomPos = GetRandomPosition(colliderBounds);

            spawnedPositions.Add(randomPos);

            GameObject selectedPrefab = spawnableObstacles[Random.Range(0, spawnableObstacles.Length)];
            GameObject spawnedObstacle = Instantiate(selectedPrefab, randomPos, Quaternion.identity);

            spawnedObstacle.transform.parent = obstacleParent.transform;
        }
    }
    //This modification ensures that the loop won't run indefinitely, and the script will exit after a specified number of attempts if a valid position is not found. Adjust the MaxAttempts value based on your game's requirements.
    private Vector2 GetRandomPosition(Bounds bounds)
    {
        int attempts = 0;

        while (attempts < MaxAttempts)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (IsPositionValid(randomPos))
            {
                return randomPos;
            }

            attempts++;
        }

        // Handle the case when a valid position couldn't be found after MaxAttempts
        Debug.LogWarning("Could not find a valid position after MaxAttempts");
        return Vector2.zero; // Or any default value you choose
    }

    private bool IsPositionValid(Vector2 position)
    {
        foreach (Vector2 spawnedPos in spawnedPositions)
        {
            if (Vector2.Distance(position, spawnedPos) < minDistanceBetweenObstacles)
            {
                return false;
            }
        }
        return true;
    }
}
