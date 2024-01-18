using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptics.Vibrations;
using Destructible2D;

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
    private List<GameObject> fireList = new List<GameObject>();

    public float deathVFXRadius;
    public int minFires;
    public int maxFires;
    private bool isTriggered;

    public Sprite destroyedBuilding;
    private LevelManager levelManager;

    [SerializeField] private GameObject pfDelvin;

    public int minEntities = 0; // Minimum number of entities to spawn
    public int maxEntities = 3; // Maximum number of entities to spawn

    private float spawnRadius = 1.0f; // Maximum distance from the current position

    public ObjectShakeScript shakeScript;

    [SerializeField] HittableSpriteGroup hitColor;

    bool isOnFire;
    bool hasDied;

    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;

    public AudioManagerScript audiomanager;
    public float spawnheight;
    public ScoreManagerScript scoremanager;

    //Destruction Variables
    [SerializeField] D2dFracturer fracture;
    [SerializeField] D2dRequirements destroyer;

    private void Awake()
    {
        buildingType = GetComponent<Targetable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        VibrateHaptics.Initialize();
        scoremanager = GameObject.Find("ScoreManager").GetComponent<ScoreManagerScript>();
        tempHealth = SO_enemy.health;
        buildingCollider = GetComponent<BoxCollider2D>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        civilianParent = GameObject.Find("---Civillian---");
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        hitColor = GetComponent<HittableSpriteGroup>();
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

        else
        {
            return;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (buildingType.enemyType == Targetable.EnemyType.BigBuilding)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerHandler playerHandler = collision.GetComponent<PlayerHandler>();
                if (playerHandler != null)
                {
                    if (playerHandler.isDashing)
                    {
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
        VibrateHaptics.VibrateHeavyClick();
        if (hitColor != null)
        {
            hitColor.Blink();
        }

        tempHealth -= damage;
        SpawnCivilian();
        audiomanager.playBuildingDamageFX();

        if (isOnFire != true)
        {
            SpawnFireVFX();
        }

        if (tempHealth <= 0)
        {
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
        VibrateHaptics.VibrateDoubleClick();
        inputHandler.ChargeUltimate(destructionScore);
        audiomanager.playBuildingDeathSFX();
        scoremanager.amtOfStructures += 1;
       // playDeathSFX();
        TriggerLoot();

        foreach (var fire in fireList)
        {
            fire.SetActive(false);
        }
        fireList.Clear();

        if(!hasDied)
        {
            hasDied = true;
        }

        buildingCollider.enabled = false;
        SpawnDeathVFX();
        spriteRenderer.sprite = destroyedBuilding;
        spriteRenderer.sortingOrder = 1;
        Invoke("StopVibration", 1f);
    }

    void StopVibration()
    {
        VibrateHaptics.Release();
    }

    public void SpawnDeathVFX()
    {
        Vector2 explosionLoc = new Vector2(transform.position.x, transform.position.y + 1.5f);
        ObjectPooler.Instance.SpawnFromPool("FireExplosionA", explosionLoc, Quaternion.identity);
        ObjectPooler.Instance.SpawnFromPool("CrumbleSmoke", transform.position, Quaternion.identity);
        ObjectPooler.Instance.SpawnFromPool("BlackSmoke",transform.position, Quaternion.Euler(-90, 0, 0));
    }
    public void SpawnFireVFX()
    {
        if (!isTriggered)
        {
            // Randomize the number of fires within a range
            int numberOfFires = Random.Range(minFires, maxFires + 1);
            fireList.Clear();
            for (int i = 0; i < numberOfFires; i++)
            {
                isTriggered = true;
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + 0.5f);
                Vector3 randomPosition = spawnLoc + Random.insideUnitSphere * deathVFXRadius;
                fireList.Add(ObjectPooler.Instance.SpawnFromPool("WreckageFlame", randomPosition, Quaternion.Euler(-90, 0, 0)));
            }
        }
    }
    void TriggerLoot()
    {
        //Add points
        levelManager.CalculateScore(destructionScore);
        
        if (buildingType.enemyType == Targetable.EnemyType.BigBuilding)
        {
            scoremanager.bigbuildingKilled += 1;
            scoremanager.goldearned += 10;
        }

        if (buildingType.enemyType == Targetable.EnemyType.Building)
        {
            scoremanager.smallbuildingKilled += 1;
            scoremanager.goldearned += 5;
        }
    }

    void DamageEffect()
    {
        if (shakeScript != null)
        {
            shakeScript.StartShake();
        }
        ObjectPooler.Instance.SpawnFromPool("DebrisHit", transform.position, Quaternion.identity);
        ObjectPooler.Instance.SpawnFromPool("HitMiscA", transform.position, Quaternion.identity);
        spriteRenderer.sprite = damagedSprite;
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
            GameObject civilian = Instantiate(pfDelvin, spawnPos, Quaternion.Euler(0f,0f,randomRotation));
            civilian.GetComponent<FakeHeightScript>().Initialize(randomDirection * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y), Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y));
            civilian.GetComponent<FakeHeightScript>().spawnerReference = this.gameObject;
           
            //Sets the civilian state upon initialization
            civilian.GetComponentInChildren<Civilian>().enemyState = Civilian.EnemyState.fall;
            civilian.transform.SetParent(civilianParent.transform);
            civilian.GetComponentInChildren<Civilian>().entityCollider.enabled = false;
        }

    }

}