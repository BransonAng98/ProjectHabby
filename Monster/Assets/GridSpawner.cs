using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{

    public List<GameObject> gridPrefabs = new List<GameObject>(); // Array of grid prefabs
    public float gridSpeed = 5f; // Speed at which the grids will move
    public float maxgridSpeed = 40f;
    public float playerDistanceTravelled = 0f; // Total distance traveled by the player
    public int Checkpoint;

    public float accelerationDuration = 30f; // Duration over which speed increases
    private float accelerationTimer = 0f; // Timer for acceleration duration

    [SerializeField] int numberOfColumns; // Number of columns in the grid
    [SerializeField] int numberOfRows;    // Number of rows in the grid

    public ERScoreManager erSM;

    public float spacingX = 50.0f;   // Horizontal spacing between prefabs
    public float spacingY = 50.0f;   // Vertical spacing between prefabs

    [SerializeField] bool isSpawned;
    [SerializeField] bool isTriggered;
    public List<GameObject> activeGrids = new List<GameObject>(); // List to keep track of active grids

    public PlayerEndlessRunnerController player;
    private float[] prefabProbabilities = { 0.4f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.2f, 0.1f, 0.2f, 0.1f };

    void Start()
    {

        erSM = GameObject.Find("ScoreManager").GetComponent<ERScoreManager>();
        isSpawned = true;
        if (isSpawned == true)
        {
            SpawnStartingGrid();
            isSpawned = false;
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEndlessRunnerController>();
    }

    void Update()
    {
        playerDistanceTravelled += gridSpeed * Time.deltaTime;
        if (player.canMove)
        {
            isTriggered = false;
            erSM.DistanceTravelled = Mathf.RoundToInt(playerDistanceTravelled);
            MoveGrids();
            // Update grid speed
            if (gridSpeed < maxgridSpeed)
            {
                accelerationTimer += Time.deltaTime;
                gridSpeed = Mathf.Lerp(0f, maxgridSpeed, accelerationTimer / accelerationDuration);
            }
        }

        else
        {
            if (player.isCCed)
            {
                DecreaseGridSpeed();
            }

            else
            {
                return;
            }
        }

        if (Checkpoint == 4)
        {
            SpawnGrid();
        }

        if (isSpawned == false)
        {
            numberOfColumns = 1;
            numberOfRows = 3;
        }
    }

    void DecreaseGridSpeed()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            float decreaseAmt = maxgridSpeed * 0.1f;
            gridSpeed -= decreaseAmt;
            accelerationTimer *= 0.6f;

            if (gridSpeed < 0)
            {
                gridSpeed = 1f;
            }

            else
            {
                return;
            }
        }
    }

    void SpawnStartingGrid()
    {
        numberOfColumns = 1;
        numberOfRows = 4;
        Checkpoint = 3;
        SpawnGrid();
    }

    void SpawnGrid()
    {
        // Calculate the initial spawn position based on the last spawned grid
        Vector3 initialSpawnPosition = Vector3.zero; // Default initial position
        if (activeGrids.Count > 0)
        {
            // Get the last spawned grid's position and adjust the spawn position accordingly
            GameObject lastGrid = activeGrids[0]; // Get the last grid
            float yOffset = spacingY * (numberOfRows - 6); // Calculate the vertical offset based on the number of rows
            initialSpawnPosition = lastGrid.transform.position + new Vector3(0f, -yOffset, 0f);
        }

        // Adjust checkpoint
        Checkpoint -= 3;

        int previousPrefabIndex = -1; // Initialize with an invalid index

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                int indexToInstantiate = row * numberOfColumns + col;

                if (indexToInstantiate < gridPrefabs.Count)
                {
                    // Weighted random selection (excluding the previously spawned prefab)
                    float randomValue = Random.value;
                    float cumulativeProbability = 0f;
                    int selectedPrefabIndex = -1;

                    for (int i = 0; i < gridPrefabs.Count; i++)
                    {
                        if (i != previousPrefabIndex) // Skip the previously spawned prefab
                        {
                            cumulativeProbability += prefabProbabilities[i];
                            if (randomValue <= cumulativeProbability)
                            {
                                selectedPrefabIndex = i;
                                break;
                            }
                        }
                    }

                    if (selectedPrefabIndex != -1)
                    {
                        GameObject prefabToInstantiate = gridPrefabs[selectedPrefabIndex];
                        // Calculate the position based on the row and column
                        Vector3 spawnPosition = initialSpawnPosition + new Vector3(col * spacingX, -row * spacingY, 0f);
                        GameObject instantiatedGrid = Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
                        activeGrids.Add(instantiatedGrid); // Add the instantiated grid to the activeGrids list

                        previousPrefabIndex = selectedPrefabIndex; // Update the previously spawned prefab index
                    }
                    else
                    {
                        Debug.LogWarning("Not enough prefabs in the list to fill the grid.");
                    }
                }
                else
                {
                    Debug.LogWarning("Not enough prefabs in the list to fill the grid.");
                }
            }
        }
    }

    void MoveGrids()
    {
        foreach (GameObject grid in activeGrids)
        {
            // Move the grid upwards
            grid.transform.Translate(Vector2.down * gridSpeed * Time.deltaTime);
        }
    }
}