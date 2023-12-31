using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;
    public int pointsforSpawning = 0;
    public int numberofEnemies;
    public int NumberofWaves;
    public float CityDestructionLevel;
    public LevelManagerScriptableObject levelData;
    public Transform playerTransform;
    public float playerSpawnRadius = 10f;
    public List<BonusEnemy> bonusenemies = new List<BonusEnemy>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public List<SpawnedEnemy> separateSpawnedEnemies = new List<SpawnedEnemy>();
    public LevelManager LevelManagerScript;

    // ...


    // The minimum and maximum positions where enemies can spawn
    public Vector2 minSpawnPosition;
    public Vector2 maxSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        CityDestructionLevel = levelData.destructionLevel;

        GenerateWave();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CityDestructionLevel = levelData.destructionLevel;

        // Check if the CityDestructionLevel and currWave meet the conditions for spawning
        if (!((CityDestructionLevel == 0 && currWave >= 1 && currWave <= 3) ||
              (CityDestructionLevel == 1 && currWave >= 4 && currWave <= 6) ||
              (CityDestructionLevel == 2 && currWave >= 7 && currWave <= 10)))
        {
            // CityDestructionLevel and currWave conditions are not met, so skip to the lowest wave in the next city destruction level.
            if (CityDestructionLevel == 0)
            {
                currWave = 1;
            }
            else if (CityDestructionLevel == 1)
            {
                currWave = 4;
                spawnBonusWave();
                Debug.Log("BonusWave1Spawned");
            }
            else if (CityDestructionLevel == 2)
            {
                currWave = 7;
                spawnBonusWave();
                Debug.Log("BonusWave2Spawned");
            }

            GenerateWave(); // Generate the next wave
            return;
        }

        if (currWave < NumberofWaves && spawnTimer <= 0 && spawnedEnemies.Count == 0)
        {
            // Check if there are enemies to spawn
            if (enemiesToSpawn.Count > 0)
            {
                // Loop through the enemies to spawn and spawn them all
                foreach (GameObject enemyPrefab in enemiesToSpawn)
                {
                    // Choose a random spawn position within the specified range
                    Vector2 randomSpawnPosition = GetRandomSpawnPosition();
                    GameObject enemy = Instantiate(enemyPrefab, randomSpawnPosition, Quaternion.identity);
                    spawnedEnemies.Add(enemy);
                }

                // Clear the enemiesToSpawn list
                enemiesToSpawn.Clear();

                // Reset the spawn timer
                spawnTimer = spawnInterval;
            }
            else
            {
                waveTimer = 0;
            }
        }

        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        // Check for missing enemies and remove them from the spawnedEnemies list
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        // Check if the wave is complete (all enemies of this wave are destroyed)
        if (waveTimer <= 0 && spawnedEnemies.Count == 0)
        {
            currWave++;
            GenerateWave();
        }

        //Waveno.text = "Wave " + currWave;
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomSpawnPosition;
        do
        {
            // Generate a random position within the specified range
            randomSpawnPosition = new Vector2(
                transform.position.x + Random.Range(minSpawnPosition.x, maxSpawnPosition.x),
                transform.position.y + Random.Range(minSpawnPosition.y, maxSpawnPosition.y)
            );
        } while (Vector2.Distance(randomSpawnPosition, playerTransform.position) < playerSpawnRadius);

        return randomSpawnPosition;
    }

    public void GenerateWave()
    {
        // Determine the wave range based on the CityDestructionLevel
        int minWave = 1;
        int maxWave = 3; // Default range for CityDestructionLevel 0

        if (levelData.destructionLevel == 1)
        {
            minWave = 4;
            maxWave = 6;
        }

        if (levelData.destructionLevel == 2)
        {
            minWave = 7;
            maxWave = 10;
        }

        // Ensure that currWave stays within the specified range
        if (currWave < minWave || currWave > maxWave)
        {
            currWave = minWave;
        }

        switch (currWave)
        {
            case 1:
                waveValue = 0;
                break;
            case 2:
                waveValue = 0;
                break;
            case 3:
                waveValue = 0;
                break;
            case 4:
                waveValue = 10;
                break;
            case 5:
                waveValue = 20;
                break;
            case 6:
                waveValue = 30;
                break;
            case 7:

                waveValue = 40;
                break;
            case 8:
                waveValue = 45;
                break;
            case 9:
                waveValue = 60;
                break;
            case 10:
                waveValue = 70;
                break;
            default:
                // Set a default wave value for other waves
                waveValue = 0; // Change this value as needed
                break;
        }
        pointsforSpawning = waveValue;

        GenerateEnemies();

        // Calculate spawn interval based on your desired rate
        spawnInterval = 1.0f; // Example: Spawn an enemy every  seconds
        waveTimer = waveDuration;
    }

    public void GenerateEnemies()
    {
        List<SpawnedEnemy> generatedEnemies = new List<SpawnedEnemy>();

        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId;

            if (currWave >= 4 && currWave <= 6)
            {
                // If currWave is between 4 and 6, only select enemies of type A
                randEnemyId = 0; // Assuming type A enemies are in the first half of the list
            }
            else if (currWave >= 7 && currWave <= 10)
            {
                // If currWave is between 7 and 10, select enemies from both types A and B
                randEnemyId = Random.Range(0, enemies.Count);
            }
            else
            {
                // Handle other cases if needed (e.g., currWave < 4)
                randEnemyId = Random.Range(0, enemies.Count);
            }

            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                GameObject enemyPrefab = enemies[randEnemyId].enemyPrefab;

                // Check if this enemy type already exists in the list
                SpawnedEnemy existingEnemy = generatedEnemies.Find(e => e.enemyPrefab == enemyPrefab);

                if (existingEnemy != null)
                {
                    // If it exists, increment the count
                    existingEnemy.count++;
                }
                else
                {
                    // If it doesn't exist, add it to the list
                    generatedEnemies.Add(new SpawnedEnemy
                    {
                        enemyPrefab = enemyPrefab,
                        count = 1
                    });

                    generatedEnemies.Remove(new SpawnedEnemy
                    {
                        enemyPrefab = enemyPrefab,
                        count = -1
                    });
                }

                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }

        enemiesToSpawn.Clear();

        // Convert the List of SpawnedEnemy to a List of GameObjects
        foreach (SpawnedEnemy spawnedEnemy in generatedEnemies)
        {
            for (int i = 0; i < spawnedEnemy.count; i++)
            {
                enemiesToSpawn.Add(spawnedEnemy.enemyPrefab);
            }
        }
    }

    void spawnBonusWave()
    {
        foreach (var bonusEnemy in bonusenemies)
        {
            for (int i = 0; i < bonusEnemy.Number; i++)
            {
                // Adjust spawn positions as needed.
                Vector2 spawnPosition = new Vector2(
                       transform.position.x + Random.Range(minSpawnPosition.x, maxSpawnPosition.x),
                       transform.position.y + Random.Range(minSpawnPosition.y, maxSpawnPosition.y)
                   );

                // Instantiate the bonus enemy prefab at the chosen position.
                Instantiate(bonusEnemy.BonusEnemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    [System.Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public int cost;
    }

    [System.Serializable]
    public class SpawnedEnemy
    {
        public GameObject enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class BonusEnemy
    {
        public GameObject BonusEnemyPrefab;
        public int Number;
    }
}