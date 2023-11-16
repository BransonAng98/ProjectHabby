using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    public PlayerHandler inputHandler; 
    public GameObject impactSprite;
    public GameObject smokeVFX;

    public GameObject deathVFX; // The VFX prefab you want to spawn
    public float deathVFXRadius; // The radius in which VFX will be spawned
    public int numberOfVFX = 3; // Number of VFX to spawn

    [SerializeField] private bool isTriggered;

    private void Start()
    {
        isTriggered = false;
    }

    public void SpawnImpactAtFoot(int footIndex)
    {
        if (footIndex >= 0 && footIndex < inputHandler.legLocations.Length)
        {
            GameObject foot = inputHandler.legLocations[footIndex];
            Vector2 footPos = new Vector2(foot.transform.position.x, foot.transform.position.y);
            Instantiate(impactSprite, footPos, Quaternion.identity);
            Instantiate(smokeVFX, footPos, Quaternion.identity);
        }
    }

    public void SpawnDeathVFX()
    {
        if (!isTriggered)
        {
            for (int i = 0; i < numberOfVFX; i++)
            {
                isTriggered = true;
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + 2f);
                Vector3 randomPosition = spawnLoc + Random.insideUnitSphere * deathVFXRadius;
                Instantiate(deathVFX, randomPosition, Quaternion.identity);
            }
        }
    }
}

