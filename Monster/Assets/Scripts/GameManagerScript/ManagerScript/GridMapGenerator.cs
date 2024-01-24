using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMapGenerator : MonoBehaviour
{

    public List<GameObject> Neighbourhood = new List<GameObject>();
    public int numberOfColumns ; // Number of columns in the grid
    public int numberOfRows;    // Number of rows in the grid

    public float spacingX = 50.0f;   // Horizontal spacing between prefabs
    public float spacingY = 50.0f;   // Vertical spacing between prefabs
    public List<GameObject> obstacleList = new List<GameObject>();

    public int prefab0Count;
    public int prefab1Count;
    public int prefab2Count;
    public int prefab3Count;
    public int prefab4Count;
    public int prefab5Count;
    public int prefab6Count;
    public int prefab7Count;
    public int prefab8Count;
    public int prefab9Count;
    public int prefab10Count;
    public int prefab11Count;
    public int prefab12Count;
    public int prefab13Count;
    public int prefab14Count;
    public int prefab15Count;
    public int prefab16Count;

    // Start is called before the first frame update
    public void Start()
    {
        generateMap();
        AstarPath.active.Scan(); //scan the grid
        ScanAndInsert();
        DisableObstacles();
      
    }

    void ScanAndInsert()
    {
        GameObject[] obstacleCollider = GameObject.FindGameObjectsWithTag("Obstacle");
        
        foreach ( var obs in obstacleCollider)
        {
            obstacleList.Add(obs);    
        }
    }

    void generateMap()
    {
        // Create an empty parent object
        GameObject mapParent = new GameObject("MapParent");

        int totalTiles = numberOfRows * numberOfColumns;

        // Create a list to store all available prefab indices
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < Neighbourhood.Count; i++)
        {
            int count = 0;
            if (i == 0)
                count = prefab0Count;
            else if (i == 1)
                count = prefab1Count;
            else if (i == 2)
                count = prefab2Count;
            else if (i == 3)
                count = prefab3Count;
            else if (i == 4)
                count = prefab4Count;
            else if (i == 5)
                count = prefab5Count;
            else if (i == 6)
                count = prefab6Count;
            else if (i == 7)
                count = prefab7Count;
            else if (i == 8)
                count = prefab8Count;
            else if (i == 9)
                count = prefab9Count;
            else if (i == 10)
                count = prefab10Count;
            else if (i == 11)
                count = prefab11Count;
            else if (i == 12)
                count = prefab12Count;
            else if (i == 13)
                count = prefab13Count;
            else if (i == 14)
                count = prefab14Count;
            else if (i == 15)
                count = prefab15Count;
            else if (i == 16)
                count = prefab16Count;
            // Add more conditions for additional prefabs if needed

            int maxCount = Mathf.Min(totalTiles, count); // Ensure we don't exceed the totalTiles
            availableIndices.AddRange(Enumerable.Repeat(i, maxCount));
        }

        // Shuffle the list to randomize the order
        for (int i = availableIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[j];
            availableIndices[j] = temp;
        }

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                if (availableIndices.Count == 0)
                {
                    // If you've run out of available indices, shuffle the list and continue
                    for (int i = 0; i < Neighbourhood.Count; i++)
                    {
                        int count = 0;
                        if (i == 0)
                            count = prefab0Count;
                        else if (i == 1)
                            count = prefab1Count;
                        else if (i == 2)
                            count = prefab2Count;
                        else if (i == 3)
                            count = prefab3Count;
                        else if (i == 4)
                            count = prefab4Count;
                        else if (i == 5)
                            count = prefab5Count;
                        else if (i == 6)
                            count = prefab6Count;
                        else if (i == 7)
                            count = prefab7Count;
                        else if (i == 8)
                            count = prefab8Count;
                        else if (i == 9)
                            count = prefab9Count;
                        else if (i == 10)
                            count = prefab10Count;
                        else if (i == 11)
                            count = prefab11Count;
                        else if (i == 12)
                            count = prefab12Count;
                        else if (i == 13)
                            count = prefab13Count;
                        else if (i == 14)
                            count = prefab14Count;
                        else if (i == 15)
                            count = prefab15Count;
                        else if (i == 16)
                            count = prefab16Count;
                        // Add more conditions for additional prefabs if needed

                        int maxCount = Mathf.Min(totalTiles, count);
                        availableIndices.AddRange(Enumerable.Repeat(i, maxCount));
                    }

                    // Shuffle the list to randomize the order
                    for (int i = availableIndices.Count - 1; i > 0; i--)
                    {
                        int j = Random.Range(0, i + 1);
                        int temp = availableIndices[i];
                        availableIndices[i] = availableIndices[j];
                        availableIndices[j] = temp;
                    }
                }

                int indexToInstantiate = availableIndices[0]; // Get the first index from the shuffled list
                availableIndices.RemoveAt(0); // Remove the used index

                GameObject prefabToInstantiate = Neighbourhood[indexToInstantiate];

                // Calculate the position based on the row and column
                Vector3 spawnPosition = new Vector3(col * spacingX, row * spacingY, 0f);

                // Instantiate the prefab as a child of the mapParent
                GameObject instantiatedObject = Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
                instantiatedObject.transform.parent = mapParent.transform;
            }
        }
    }


    void DisableObstacles()
    {
        foreach (var obs in obstacleList)
        {
            Destroy(obs);
        }

    }
}

