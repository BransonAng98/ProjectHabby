using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameLandmark : MonoBehaviour
{

    public EnemyScriptableObject enemyData;
    ObjectShakeScript shakeLandmark;
    private float health;


    [SerializeField] GameObject pfDelvin;
    public GameObject parentObject;
    [SerializeField] float healthPercentage;
    [SerializeField] int healthState;
    [SerializeField] bool isDead;
    public int minEntities = 1; // Minimum number of entities to spawn
    public int maxEntities = 4; // Maximum number of entities to spawn
    public float spawnRadius = 3.0f; // Maximum distance from the current position
    public float sinkingSpeed;

    public List<Sprite> landmarkSprites = new List<Sprite>();
    private SpriteRenderer spriteRenderer;
    private float hitDarkeningAmount = 0.2f; // Amount to darken the sprite on each hit
    private float minDarkness = 0.0f; // Minimum darkness level

    public AudioSource landmarkAudioSource;

    public AudioClip[] landmarkSFX;

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
            playDamageSFX();
            health -= damage;
            shakeLandmark.StartShake();
            parentObject.transform.Translate(Vector3.down * sinkingSpeed);
            SpawnCivilian();
            DamageEffect();
            UpdateSprite();
        }

        else
        {
            playDeathSFX();
            Death();
        }
    }

    private void SinkLandmark()
    {
        float sinkAmount = 5f * Time.deltaTime;
        parentObject.transform.Translate(Vector3.down * sinkAmount);
    }

    private void Death()
    {
        if (gameObject != null)
        {
            isDead = true;
            Destroy(gameObject, 2f);
            // Send back to the level select
            Invoke("ReturnToLevelSelect", 6f);
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

    void UpdateSprite()
    {
        if(healthState == 0)
        {
            healthState++;
        }

        if(healthPercentage <50)
        {
            if(healthState == 1)
            {
                healthState += 1;
                //change to first damage sprite
            }
        }

        if(healthPercentage < 0)
        {
            if (healthState == 2)
            {
                //Change to destroyed sprite
            }
        }
    }

    float CheckHealthPercentage()
    {
        return Mathf.Clamp01(health / enemyData.health) * 100f;
    }

    // Update is called once per frame
    void Update()
    {
        healthPercentage = CheckHealthPercentage();

        if (isDead)
        {
            SinkLandmark();
        }

        else
        {
            return;
        }
    }

    public void playDamageSFX()
    {
        AudioClip damagesoundtoPlay = landmarkSFX[Random.Range(0, 3)];
        landmarkAudioSource.PlayOneShot(damagesoundtoPlay);
        Debug.Log("PlaySound");
    }

    public void playDeathSFX()
    {
        AudioClip damagesoundtoPlay = landmarkSFX[Random.Range(4,6)];
        landmarkAudioSource.PlayOneShot(damagesoundtoPlay);
        Debug.Log("PlaySound");
    }
}
