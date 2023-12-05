using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    public PlayerHandler inputHandler; 
    public GameObject impactVFX;
    public GameObject aoeVFX;
    public GameObject ultiRdyVFX;
    public GameObject ultimateVFX;
    public GameObject deathVFX;
    public GameObject dashFootVFX;
    public GameObject upgradeVFX;
    public GameObject dashBodyVFX;
    
    private GameObject player;
    
    public float deathVFXRadius; 
    public float footTremorRadius;
    public float aoeTremorRadius;
    public int numberOfVFX = 3; 
    
    public GameObject[] legLocations;

    [SerializeField] private bool isTriggered;
    public bool isDashing;

    private PlayerHandler playerHandler;

    private void Start()
    {
        playerHandler = GetComponent<PlayerHandler>();
        isTriggered = false;
    }

    public void footImpact(int foot)
    {
        Vector2 correction = new Vector2(legLocations[foot].transform.position.x, legLocations[foot].transform.position.y);
      
        if (!isDashing)
        {
            Instantiate(impactVFX, correction, Quaternion.identity);
        }
        else 
        {
            Instantiate(dashFootVFX, correction, Quaternion.identity);
        }
       
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(correction, footTremorRadius);
        foreach (Collider2D colldier in hitColliders)
        {
            if (colldier.CompareTag("Tree"))
            {
                ObjectShakeScript tree = colldier.GetComponent<ObjectShakeScript>();
                tree.StartShake();
            }
        }
    }

    public void TriggerAoeTremor()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, aoeTremorRadius);
        foreach (Collider2D colldier in hitColliders)
        {
            if (colldier.CompareTag("Tree"))
            {
                ObjectShakeScript tree = colldier.GetComponent<ObjectShakeScript>();
                tree.StartShake();
            }
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

    public void SpawnAoeVFX()
    {
        Vector2 aoePos = new Vector2(transform.position.x, transform.position.y - 1f);
        GameObject aoe = Instantiate(aoeVFX, aoePos, Quaternion.identity);
    }

    public void SpawnUltiVFX()
    {
        Vector2 ultiPos = new Vector2(this.transform.position.x, this.transform.position.y -2.5f);
        GameObject ultiVFX = Instantiate(ultimateVFX, ultiPos, Quaternion.identity);
    }

    public void SpawnUpgradeVFX()
    {
        Vector2 upgradePos = new Vector2(this.transform.position.x, this.transform.position.y + 2f);
        Instantiate(upgradeVFX, upgradePos, Quaternion.identity);
    }

}

