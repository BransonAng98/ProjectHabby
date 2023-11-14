using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    /*[SerializeField] public GameObject[] spawnableObstacles; // Public array to hold different prefabs.
    [SerializeField] private int minNumberOfObjectsToSpawn;
    [SerializeField] private int maxNumberOfObjectsToSpawn; //don't put it over 3. Idk why
    [SerializeField] private float minDistanceBetweenObstacles; // Minimum distance between spawned obstacles
    public GameObject obstacleparent;

    private List<Vector2> spawnedPositions = new List<Vector2>();

    private void Start()
    {
        obstacleparent = GameObject.Find("---Obstacles---");
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        foreach (BoxCollider2D boxCollider in colliders)
        {
            Bounds colliderBounds = boxCollider.bounds;

            int numberOfObjectsToSpawn = Random.Range(minNumberOfObjectsToSpawn, maxNumberOfObjectsToSpawn + 1);

            GameObject selectedPrefab = spawnableObstacles[Random.Range(0, spawnableObstacles.Length)];
            for (int i = 0; i < numberOfObjectsToSpawn; i++)
            {
                if (Random.value > 0.25f)
                {
                    continue;
                }

                Vector2 randomPos = Vector2.zero;
                bool positionValid = false;

                while (!positionValid)
                {
                    randomPos = new Vector2(
                        Random.Range(colliderBounds.min.x, colliderBounds.max.x),
                        Random.Range(colliderBounds.min.y, colliderBounds.max.y)
                    );

                    positionValid = IsPositionValid(randomPos);
                }

                spawnedPositions.Add(randomPos);

                // Randomly select a prefab from the spawnableObstacles array.
                GameObject spawnedObstacle = Instantiate(selectedPrefab, randomPos, Quaternion.identity);

                spawnedObstacle.transform.parent = obstacleparent.transform;
            }
        }
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
    }*/
}

