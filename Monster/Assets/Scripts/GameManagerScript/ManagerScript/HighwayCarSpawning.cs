using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayCarSpawning : MonoBehaviour
{
    public GameObject cartoSpawn;
    public float spawnInterval = 2f; // Time interval between spawns in seconds
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if it's time to spawn a new object
        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f; // Reset the timer
        }
    }
    void SpawnObject()
    {
        // Instantiate a new object at the spawner's position and rotation
        Instantiate(cartoSpawn, transform.position, Quaternion.identity);
    }
}
