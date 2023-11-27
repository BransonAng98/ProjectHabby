using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    public PlayerHandler inputHandler; 
    public GameObject impactSprite;
    public GameObject smokeVFX;
    public GameObject aoeVFX;
    public GameObject ultiRdyVFX;
    public GameObject ultimateVFX;
    public GameObject arrowIndicator;
    private GameObject player;

    public GameObject deathVFX; // The VFX prefab you want to spawn
    public float deathVFXRadius; // The radius in which VFX will be spawned
    public int numberOfVFX = 3; // Number of VFX to spawn

    public GameObject[] legLocations;

    [SerializeField] private bool isTriggered;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isTriggered = false;

    }

    public void footImpact(int foot)
    {
        Vector2 correction = new Vector2(legLocations[foot].transform.position.x, legLocations[foot].transform.position.y - 1f);
        Instantiate(impactSprite, correction, Quaternion.identity);
        Instantiate(smokeVFX, correction, Quaternion.identity);
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

    public void SpawnAoeVFX()
    {
        Vector2 aoePos = new Vector2(transform.position.x, transform.position.y - 1f);
        GameObject aoe = Instantiate(aoeVFX, aoePos, Quaternion.identity);
    }

    public void SpawnUltiVFX()
    {
        Vector2 ultiPos = new Vector2(player.transform.position.x, player.transform.position.y - 0.8f);
        GameObject ultiVFX = Instantiate(ultimateVFX, ultiPos, Quaternion.identity);
    }
}

