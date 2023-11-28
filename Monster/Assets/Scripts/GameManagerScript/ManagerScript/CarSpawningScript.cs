using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawningScript : MonoBehaviour
{
    [SerializeField] public GameObject[] spawnableCars; // Public array to hold different prefabs.
    [SerializeField] private int minNumberOfObjectsToSpawn;
    [SerializeField] private int maxNumberOfObjectsToSpawn; //don't put it over 3. Idk why
    [SerializeField] private float minDistanceBetweenCars; // Minimum distance between spawned cars
    public GameObject carparent;

    private List<Vector2> spawnedPositions = new List<Vector2>();

    private void Start()
    {
        carparent = GameObject.Find("---CARS---");
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        foreach (BoxCollider2D boxCollider in colliders)
        {
            Bounds colliderBounds = boxCollider.bounds;

            int numberOfObjectsToSpawn = Random.Range(minNumberOfObjectsToSpawn, maxNumberOfObjectsToSpawn + 1);

            for (int i = 0; i < numberOfObjectsToSpawn; i++)
            {
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

                // Check if the collider's size in the Y-axis is greater than 10.
                bool spawnVertically = colliderBounds.size.y > 10;

                // Randomly select a prefab from the spawnableCars array.
                GameObject randomPrefab = spawnableCars[Random.Range(0, spawnableCars.Length)];
                GameObject spawnedCar = Instantiate(randomPrefab, randomPos, Quaternion.identity);

                if (spawnVertically)
                {
                    CarAI carAI = randomPrefab.GetComponentInChildren<CarAI>();
                    if (carAI != null)
                    {
                        carAI.SetSpriteRenderer(spawnedCar.GetComponentInChildren<SpriteRenderer>());
                        carAI.SetSpriteUp();
                        
                    }
                }
                spawnedCar.transform.parent = carparent.transform;
            }
        }
    }

    private bool IsPositionValid(Vector2 position)
    {
        foreach (Vector2 spawnedPos in spawnedPositions)
        {
            if (Vector2.Distance(position, spawnedPos) < minDistanceBetweenCars)
            {
                return false;
            }
        }
        return true;
    }
}
