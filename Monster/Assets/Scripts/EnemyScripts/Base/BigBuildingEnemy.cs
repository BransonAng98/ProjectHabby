using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBuildingEnemy : MonoBehaviour
{
    public EnemyScriptableObject SO_enemy;
    public float tempHealth;
    public SpriteRenderer spriteRenderer;
    public Sprite damagedSprite;
    public Targetable buildingType;
    private Collider2D buildingCollider;
    public PlayerHandler inputHandler;
    public GameObject civilianParent;
    public int destructionScore = 5;

    //VFX
    public GameObject fireVFX;
    public GameObject deathVFX;
    public GameObject damageVFX;
    public GameObject crumblingVFX;
    public GameObject hitVFX;
    public GameObject smokeVFX;
    public GameObject pointIndicatorVFX;
    private GameObject fireHandler;

    public Sprite destroyedBuilding;
    private LevelManager levelManager;
    private EventManager eventManager;
    private Vector3 targetScale = new Vector3(2f, 0, 0);

    [SerializeField] private GameObject pfCoin;
    [SerializeField] private GameObject pfDelvin;

    public int minEntities = 1; // Minimum number of entities to spawn
    public int maxEntities = 4; // Maximum number of entities to spawn
    public int minCoins = 1;
    public int maxCoins = 4;
    public float spawnRadius = 3.0f; // Maximum distance from the current position

    public ObjectShakeScript shakeScript;

    bool isOnFire;
    bool hasDied;

    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;

    public AudioSource buildingAudioSource;
    public AudioClip[] damageSFX;
    public AudioClip[] deathSFX;

    private void Awake()
    {
        buildingType = GetComponent<Targetable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tempHealth = SO_enemy.health;
        buildingCollider = GetComponent<BoxCollider2D>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        civilianParent = GameObject.Find("---Civillian---");
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        buildingAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(buildingType.enemyType == Targetable.EnemyType.Building)
        {
            if (collision.CompareTag("PlayerLeg"))
            {
                TakeDamage(10);
            }
        }

        if(buildingType.enemyType == Targetable.EnemyType.BigBuilding)
        {
            return;
        }

        else
        {
            return;
        }
    }

    public void TakeDamage(float damage)
    {
        tempHealth -= damage;
        if(shakeScript != null)
        {
            shakeScript.StartShake();
        }
        SpawnCivilian();
        DamageEffect();
        playDamageSFX();

        if (isOnFire != true)
        {
            SpawnFire();
        }

        if (tempHealth <= 0)
        {
            inputHandler.ChargeUltimate(10);
            Death();
        }
        else return;
    }

    void SpawnFire()
    {
        GameObject fireAnim = Instantiate(fireVFX, transform.position, Quaternion.identity);
        fireHandler = fireAnim;
        isOnFire = true;
    }

    public void Death()
    {
        playDeathSFX();
        Destroy(fireHandler);
        TriggerLoot();
        
        if(!hasDied)
        {
            eventManager.AddScore();
            hasDied = true;
        }

        buildingCollider.enabled = false;
        SpawnDeathVFX();
        spriteRenderer.sprite = destroyedBuilding;
        spriteRenderer.sortingOrder = 2;
 
    }


    public void SpawnDeathVFX()
    {
        Vector2 explosionLoc = new Vector2(transform.position.x, transform.position.y + 1.5f);
        GameObject explosion = Instantiate(deathVFX, explosionLoc, Quaternion.identity);
        GameObject crumble = Instantiate(crumblingVFX, transform.position, Quaternion.identity);
        GameObject smoke = Instantiate(smokeVFX, transform.position, Quaternion.Euler(-90, 0, 0)); 
  
    }

    void TriggerLoot()
    {

        int numberOfEntities = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < numberOfEntities; i++)
        {
            //Spawn GNA
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            GameObject coin = Instantiate(pfCoin, transform.position, Quaternion.identity);
            coin.transform.Rotate(0, 0, 90);
        }
        //Add points
        levelManager.CalculateScore(destructionScore);
        GameObject pointVFX = Instantiate(pointIndicatorVFX, transform.position, Quaternion.Euler(0f, 0f, 0f));
    }

    void DamageEffect()
    {
        GameObject hit = Instantiate(damageVFX, transform.position, Quaternion.identity);
        GameObject hitEffect = Instantiate(hitVFX, transform.position, Quaternion.identity);
        spriteRenderer.sprite = damagedSprite;
        Destroy(hit, 1f);
    }

    private void SpawnCivilian()
    {
        int numberOfEntities = Random.Range(minEntities, maxEntities + 1);
        for (int i = 0; i < numberOfEntities; i++)
        {
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPos = transform.position + randomDirection * Random.Range(0.0f, spawnRadius);
            GameObject civilian = Instantiate(pfDelvin, spawnPos, Quaternion.identity);
            civilian.GetComponent<FakeHeightScript>().Initialize(Random.insideUnitCircle * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y), Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y));

            //Sets the civilian state upon initialization
            civilian.GetComponentInChildren<Civilian>().enemyState = Civilian.EnemyState.fall;
            civilian.transform.SetParent(civilianParent.transform);
            civilian.GetComponentInChildren<Civilian>().entityCollider.enabled = false;
        }

    }

    void playDamageSFX()
    {
        AudioClip damagesoundtoPlay = damageSFX[Random.Range(0, damageSFX.Length)];
        buildingAudioSource.PlayOneShot(damagesoundtoPlay);
        Debug.Log("PlaySound");
    }

    void playDeathSFX()
    {
        AudioClip deathsoundtoPlay = deathSFX[Random.Range(0, deathSFX.Length)];
        buildingAudioSource.PlayOneShot(deathsoundtoPlay);
    }

}