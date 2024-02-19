using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{

    public List<GameObject> gridPrefabs = new List<GameObject>(); // Array of grid prefabs
    public float gridSpeed = 5f; // Speed at which the grids will move
    public float playerDistanceTravelled = 0f; // Total distance traveled by the player
    public int Checkpoint;

    [SerializeField] int numberOfColumns; // Number of columns in the grid
    [SerializeField] int numberOfRows;    // Number of rows in the grid

    public ERScoreManager erSM;

    public float spacingX = 50.0f;   // Horizontal spacing between prefabs
    public float spacingY = 50.0f;   // Vertical spacing between prefabs

    [SerializeField] bool isSpawned;
    public List<GameObject> activeGrids = new List<GameObject>(); // List to keep track of active grids

    public PlayerEndlessRunnerController player;

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
        erSM.DistanceTravelled = Mathf.RoundToInt(playerDistanceTravelled);
        if (player.canMove)
        {
            MoveGrids();
        }

        if (Checkpoint == 3)
        {
            SpawnGrid();
        }

        if(isSpawned == false)
        {
            numberOfColumns = 1;
            numberOfRows = 3;
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
            GameObject lastGrid = activeGrids[activeGrids.Count - 1]; // Get the last grid
            float yOffset = spacingY * (numberOfRows - 2); // Calculate the vertical offset based on the number of rows
            initialSpawnPosition = lastGrid.transform.position + new Vector3(0f, -yOffset, 0f);
        }

        // Adjust checkpoint
        Checkpoint -= 3;

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                int indexToInstantiate = row * numberOfColumns + col;

                if (indexToInstantiate < gridPrefabs.Count)
                {
                    int randomIndex = Random.Range(0, gridPrefabs.Count);
                    GameObject prefabToInstantiate = gridPrefabs[randomIndex]; // Select a random prefab from gridPrefabs
                                                                               // Calculate the position based on the row and column
                    Vector3 spawnPosition = initialSpawnPosition + new Vector3(col * spacingX, -row * spacingY, 0f); // Adjust y to negative row * spacingY

                    GameObject instantiatedGrid = Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
                    activeGrids.Add(instantiatedGrid); // Add the instantiated grid to the activeGrids list
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
            grid.transform.Translate(Vector2.up * gridSpeed * Time.deltaTime);
        }
    }
}
