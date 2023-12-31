using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrike : MonoBehaviour
{
    public int numberOfPlanes;
    public int numberOfMissiles;
    public GameObject enemyPlane;

    public Transform player;
    public Transform[] spawnPoints;
    [SerializeField] Transform missileSpawnPos;

    public GameObject warningZone;
    public GameObject missilePrefab;


    private void Start()
    {
        // Check if we have at least one spawn point
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        if (warningZone == null)
        {
            Debug.LogError("SpriteRenderer component not found.");
            return;
        }

    }

    public void ActivateAirStrike()
    {
        if (enemyPlane != null)
        {
            Invoke("RandomizeAndSpawn", 2f);

        }
    }

    public void RandomizeAndSpawn()
    { 
        spawnPoints[0].position = new Vector3(player.position.x, player.position.y + 20f, player.position.z);
        spawnPoints[1].position = new Vector3(player.position.x, player.position.y, player.position.z);
        spawnPoints[2].position = new Vector3(player.position.x, player.position.y - 20f, player.position.z);

        //Choose a random spawn point
        foreach (Transform pos in spawnPoints)
        {
            GameObject scapeGoat = Instantiate(warningZone, pos.position, Quaternion.identity);
            DestroyWarningZone(scapeGoat);

            float leftOrRight = Random.Range(0, 1 + 1);
            switch (leftOrRight)
            {
                case 0:
                    Vector3 spawnPointL = new Vector3(pos.position.x - 65f, pos.position.y, 0f);
                    SpawnObject(spawnPointL);
                    SpawnMissiles(false, spawnPointL);
                    break;
                case 1:
                    Vector3 spawnPointR = new Vector3(pos.position.x + 65f, pos.position.y, 0f);
                    SpawnObject(spawnPointR);
                    SpawnMissiles(true, spawnPointR);
                    break;
            }
        }
    }

    void DestroyWarningZone(GameObject zone)
    {
        Destroy(zone, 6f);
    }

    public void SpawnMissiles(bool movingLeft , Vector3 firingPoint)
    {
        // Define the stagger amount and initial offset
        float staggerAmountX = 2.0f;  // Adjust this based on desired spacing along x-axis
        float staggerAmountY = 2.0f;  // Adjust this based on desired spacing along y-axis
        float initialOffsetX = -staggerAmountX * 1.5f;  // Adjust initial offsets based on the formation
        float initialOffsetY = -staggerAmountY;  // Adjust initial offsets based on the formation

        for (int i = 0; i < numberOfPlanes; i++)
        {
            // Calculate the staggered position for each fighter plane
            float xOffset = initialOffsetX + i * staggerAmountX;
            float yOffset = initialOffsetY + i * staggerAmountY;
            Vector3 staggeredPosition = new Vector3(firingPoint.x + xOffset, firingPoint.y + yOffset, 0f);
            // Instantiate the fighter jet at the staggered position
            GameObject newMissile = Instantiate(missilePrefab, staggeredPosition, Quaternion.identity);

            if (movingLeft == true)
            {
                newMissile.GetComponent<PlaneMissileScript>().isLeft = true;
            }
            else
            {
                newMissile.GetComponent<PlaneMissileScript>().isLeft = false;
            }
        }

    }

    public void SpawnObject(Vector3 spawnPoint)
    {
        // Define the stagger amount and initial offset
        float staggerAmountX = 2.0f;  // Adjust this based on desired spacing along x-axis
        float staggerAmountY = 2.0f;  // Adjust this based on desired spacing along y-axis
        float initialOffsetX = -staggerAmountX * 1.5f;  // Adjust initial offsets based on the formation
        float initialOffsetY = -staggerAmountY;  // Adjust initial offsets based on the formation

        for (int i = 0; i < numberOfPlanes; i++)
        {
            // Calculate the staggered position for each fighter plane
            float xOffset = initialOffsetX + i * staggerAmountX;
            float yOffset = initialOffsetY + i * staggerAmountY;
            Vector3 staggeredPosition = new Vector3(spawnPoint.x + xOffset, spawnPoint.y + yOffset, 0f);

            // Instantiate the fighter jet at the staggered position
            GameObject fighterJet = Instantiate(enemyPlane, staggeredPosition, Quaternion.identity);
        }
    }
}
