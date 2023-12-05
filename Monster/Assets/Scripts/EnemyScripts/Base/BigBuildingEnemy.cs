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
    public int destructionScore;

    //VFX
    public GameObject fireVFX;
    public GameObject deathVFX;
    public GameObject damageVFX;
    public GameObject crumblingVFX;
    public GameObject hitVFX;
    public GameObject smokeVFX;
    public GameObject pointIndicatorVFX;
    private GameObject fireHandler;
    public float deathVFXRadius;
    public int minFires;
    public int maxFires;
    private bool isTriggered;
    private List<GameObject> fireHandlers = new List<GameObject>();

    public Sprite destroyedBuilding;
    private LevelManager levelManager;
    private EventManager eventManager;
    private Vector3 targetScale = new Vector3(2f, 0, 0);

    [SerializeField] private GameObject pfCoin;
    [SerializeField] private GameObject pfFallingDelvin;

    public int minEntities = 0; // Minimum number of entities to spawn
    public int maxEntities = 3; // Maximum number of entities to spawn
    public int minCoins = 1;
    public int maxCoins = 4;
    private float spawnRadius = 1.0f; // Maximum distance from the current position

    public ObjectShakeScript shakeScript;

    bool isOnFire;
    bool hasDied;

    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;

    public AudioManagerScript audiomanager;
    public float spawnheight;
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
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(buildingType.enemyType == Targetable.EnemyType.Building)
        {
            if (collision.CompareTag("PlayerLeg"))
            {
                TakeDamage(inputHandler.stepDamageHolder);
            }
        }

        if(buildingType.enemyType == Targetable.EnemyType.BigBuilding)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerHandler playerHandler = collision.GetComponent<PlayerHandler>();
                if(playerHandler != null)
                {
                    if (playerHandler.isDashing)
                    {
                        Debug.Log("Trampled");
                        TakeDamage(inputHandler.stepDamageHolder);
                    }
                    else { return; }
                }
                else { return; }
            }
            else { return; }
        }

        else
        {
            return;
        }
    }

    public void TakeDamage(float damage)
    {
        tempHealth -= damage;
        SpawnCivilian();
        audiomanager.playBuildingDamageFX();
        //playDamageSFX();

        if (isOnFire != true)
        {
            SpawnFireVFX();
        }

        if (tempHealth <= 0)
        {
            inputHandler.ChargeUltimate(destructionScore);
            Death();
            inputHandler.DisableAttack(buildingCollider);
        }
        else
        {
            DamageEffect();
        }

    }

    public void Death()
    {
        audiomanager.playBuildingDeathSFX();
       // playDeathSFX();
        TriggerLoot();

        foreach (var fireHandler in fireHandlers)
        {
            Destroy(fireHandler);
        }
        fireHandlers.Clear();

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
    public void SpawnFireVFX()
    {
        if (!isTriggered)
        {
            // Randomize the number of fires within a range
            int numberOfFires = Random.Range(minFires, maxFires + 1);

            for (int i = 0; i < numberOfFires; i++)
            {
                isTriggered = true;
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + 0.5f);
                Vector3 randomPosition = spawnLoc + Random.insideUnitSphere * deathVFXRadius;
                GameObject fireAnim = Instantiate(fireVFX, randomPosition, Quaternion.Euler(-90, 0, 0));
                fireHandlers.Add(fireAnim);
                isOnFire = true;
            }
        }
    }
    void TriggerLoot()
    {

        //int numberOfEntities = Random.Range(minCoins, maxCoins + 1);
        //for (int i = 0; i < numberOfEntities; i++)
        //{
        //    //Spawn GNA
        //    Vector2 randomDirection = Random.insideUnitCircle.normalized;
        //    GameObject coin = Instantiate(pfCoin, transform.position, Quaternion.identity);
        //    coin.transform.Rotate(0, 0, 90);
        //}
        //Add points
        levelManager.CalculateScore(destructionScore);
        Vector2 pointPos = new Vector2(transform.position.x, transform.position.y + 2f);
        GameObject pointVFX = Instantiate(pointIndicatorVFX, pointPos, Quaternion.Euler(0f, 0f, 0f));
    }

    void DamageEffect()
    {
        if (shakeScript != null)
        {
            shakeScript.StartShake();
        }
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
            // Choose between two fixed directions
            Vector3 fixedDirection1 = new Vector3(1.0f, 0.0f, 0.0f); // Example: Right direction
            Vector3 fixedDirection2 = new Vector3(-1.0f, 0.0f, 0.0f); // Example: Left direction

            Vector3 randomDirection = (Random.Range(0, 2) == 0) ? fixedDirection1 : fixedDirection2;

            Vector3 spawnPos = transform.position + new Vector3(0,spawnheight,0) + randomDirection * Random.Range(0.0f, spawnRadius);
            float randomRotation = Random.Range(0f, 360f);
            GameObject civilian = Instantiate(pfFallingDelvin, spawnPos, Quaternion.Euler(0f,0f,randomRotation));
            civilian.GetComponent<FakeHeightScript>().Initialize(randomDirection * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y), Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y));
            civilian.GetComponent<FakeHeightScript>().spawnerReference = this.gameObject;
            //Sets the civilian state upon initialization
            civilian.GetComponentInChildren<Civilian>().enemyState = Civilian.EnemyState.fall;
            civilian.transform.SetParent(civilianParent.transform);
            civilian.GetComponentInChildren<Civilian>().entityCollider.enabled = false;
        }

    }

    /*void playDamageSFX()
    {
        AudioClip damagesoundtoPlay = damageSFX[Random.Range(0, damageSFX.Length)];
        buildingAudioSource.PlayOneShot(damagesoundtoPlay);
        Debug.Log("PlaySound");
    }

    void playDeathSFX()
    {
        AudioClip deathsoundtoPlay = deathSFX[Random.Range(0, deathSFX.Length)];
        buildingAudioSource.PlayOneShot(deathsoundtoPlay);
    }*/

}