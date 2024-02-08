using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{

    public List<GameObject> gridPrefabs = new List<GameObject>(); // Array of grid prefabs
    public float gridSpeed = 5f; // Speed at which the grids will move


    public int Checkpoint;

    public int numberOfColumns = 2; // Number of columns in the grid
    public int numberOfRows = 2;    // Number of rows in the grid

    public float spacingX = 50.0f;   // Horizontal spacing between prefabs
    public float spacingY = 50.0f;   // Vertical spacing between prefabs
    [SerializeField]
    public List<GameObject> activeGrids = new List<GameObject>(); // List to keep track of active grids

    void Start()
    {
        Checkpoint = 4;
        SpawnGrid();
    }

    void Update()
    {
        MoveGrids();

        if (Checkpoint == 1)
        {
            SpawnGrid();
        }
    }



    void SpawnGrid()
    {
        // Calculate the initial spawn position based on the last spawned grid
        Vector3 initialSpawnPosition = Vector3.zero; // Default initial position
        if (activeGrids.Count > 0)
        {
            // Get the last spawned grid's position and adjust the spawn position accordingly
            GameObject lastGrid = activeGrids[0];
            initialSpawnPosition = lastGrid.transform.position - new Vector3(0f, spacingY * 4  , 0f);
        }

        // Adjust checkpoint
        Checkpoint -= 4;

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                int indexToInstantiate = row * numberOfColumns + col;

                if (indexToInstantiate < gridPrefabs.Count)
                {
                    GameObject prefabToInstantiate = gridPrefabs[indexToInstantiate];

                    // Calculate the position based on the row and column
                    Vector3 spawnPosition = initialSpawnPosition + new Vector3(col * spacingX, row * spacingY, 0f);

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
