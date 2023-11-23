using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlant : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    public float tempHealth;
    public PlayerHandler inputHandler;
    public float explosionRange;
    public int explosionDamage;
    public Sprite damagedSprite;
    public Sprite destroyedSprite;
    public int minEntities = 1; // Minimum number of entities to spawn
    public int maxEntities = 4; // Maximum number of entities to spawn
    public int destructionScore = 5;

    private LevelManager levelManager;
    private ObjectShakeScript shakeScript;
    private Collider2D collider;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject pfCoin;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject smokeVFX;
    [SerializeField] private GameObject damageVFX;
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private bool isTriggered;
    [SerializeField] private bool isOnFire;

    private GameObject fireHandler;
    public float deathVFXRadius;
    public int minFires;
    public int maxFires;
    private List<GameObject> fireHandlers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        tempHealth = enemyData.health;
        collider = GetComponent<BoxCollider2D>();
        shakeScript = GetComponent<ObjectShakeScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
    }

    public void TakeDamage(float damage)
    {
        tempHealth -= damage;
        shakeScript.StartShake();
        DamageEffect();

        if (!isOnFire)
        {
            SpawnFireVFX();
        }

        if(tempHealth <= 0)
        {
            Death();
            inputHandler.ChargeUltimate(30);
        }
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
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + 1f);
                Vector3 randomPosition = spawnLoc + Random.insideUnitSphere * deathVFXRadius;
                GameObject fireAnim = Instantiate(fireVFX, randomPosition, Quaternion.Euler(-90, 0, 0));
                fireHandlers.Add(fireAnim);
                isOnFire = true;
            }
        }
    }

    void TriggerLoot()
    {
  
        int numberOfEntities = Random.Range(minEntities, maxEntities + 1);
        for (int i = 0; i < numberOfEntities; i++)
        {
            //Spawn GNA
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            GameObject coin = Instantiate(pfCoin, transform.position, Quaternion.identity);
            coin.transform.Rotate(0, 0, 90);
        }
        //Add points
        levelManager.CalculateScore(destructionScore);
    }

    void DamageEffect()
    {
        GameObject hit = Instantiate(damageVFX, transform.position, Quaternion.identity);
        GameObject hitEffect = Instantiate(hitVFX, transform.position, Quaternion.identity);
        spriteRenderer.sprite = damagedSprite;
        Destroy(hit, 1f);
    }

    public void Death()
    {
        collider.enabled = false;
        if (!isTriggered)
        {
            TriggerLoot();
            GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRange);
            foreach(Collider2D collider in hitColliders)
            {
                if (collider.CompareTag("BigBuilding"))
                {
                    BigBuildingEnemy bigBuilding = collider.GetComponent<BigBuildingEnemy>();
                    if (bigBuilding != null)
                    {
                        bigBuilding.TakeDamage(explosionDamage);
                    }
                    else { return; }
                }

                else if (collider.CompareTag("Civilian"))
                {
                    Civilian civilian = collider.GetComponent<Civilian>();
                    if (civilian != null)
                    {
                        civilian.enemyState = Civilian.EnemyState.death;
                    }
                    else { return; }
                }


                else if (collider.CompareTag("Tree"))
                {
                    Trees tree = collider.GetComponent<Trees>();
                    if (tree != null)
                    {
                        tree.Death();
                    }
                    else { return; }
                }

                else if (collider.CompareTag("Car"))
                {
                    CarAI car = collider.GetComponent<CarAI>();
                    if (car != null)
                    {
                        car.Death();
                    }
                    else { return; }
                }

                else if (collider.CompareTag("Player"))
                {
                    PlayerHealthScript playerHp = collider.GetComponent<PlayerHealthScript>();
                    if(playerHp != null)
                    {
                        playerHp.TakeDamage(explosionDamage);
                    }
                }
            }
            isTriggered = true;
            Destroy(explosion, 1f);
            spriteRenderer.sprite = destroyedSprite;
        }
    }
}
