using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameLandmark : MonoBehaviour
{

    public EnemyScriptableObject enemyData;
    ObjectShakeScript shakeLandmark;
    private float health;


    [SerializeField] private GameObject pfDelvin;
    public int minEntities = 1; // Minimum number of entities to spawn
    public int maxEntities = 4; // Maximum number of entities to spawn
    public float spawnRadius = 3.0f; // Maximum distance from the current position

    private SpriteRenderer spriteRenderer;
    private float hitDarkeningAmount = 0.2f; // Amount to darken the sprite on each hit
    private float minDarkness = 0.0f; // Minimum darkness level

    // Start is called before the first frame update
    void Start()
    {
        health = enemyData.health;
        shakeLandmark = GetComponent<ObjectShakeScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SpawnCivilian()
    {
        int numberOfEntities = Random.Range(minEntities, maxEntities + 1);
        for (int i = 0; i < numberOfEntities; i++)
        {
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPos = transform.position + randomDirection * Random.Range(0.0f, spawnRadius);
            GameObject civilian = Instantiate(pfDelvin, spawnPos, Quaternion.identity);
            //Sets the civilian state upon initialization
        }

    }

    public void TakeDamage(float damage)
    {
        if(health >= 0)
        {
            health -= damage;
            shakeLandmark.StartShake();
            SpawnCivilian();
            DamageEffect();
        }

        else
        {
            Death();
        }
    }

    private void Death()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
            // Send back to the level select
            Invoke("ReturnToLevelSelect", 2f);
        }
        else return;
    }

    void ReturnToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    void DamageEffect()
    {
        Color currentColor = spriteRenderer.color;

        // Reduce the brightness of the sprite by the specified amount
        currentColor.r = Mathf.Max(minDarkness, currentColor.r - hitDarkeningAmount);
        currentColor.g = Mathf.Max(minDarkness, currentColor.g - hitDarkeningAmount);
        currentColor.b = Mathf.Max(minDarkness, currentColor.b - hitDarkeningAmount);

        // Apply the new color to the sprite
        spriteRenderer.color = currentColor;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
