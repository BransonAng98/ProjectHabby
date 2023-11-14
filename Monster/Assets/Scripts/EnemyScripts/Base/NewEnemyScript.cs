using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class NewEnemyScript : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    public enum EnemyState { move, attack, death }
    public EnemyState currentState;

    private Transform player;
    private Transform playerLastLoc;
    private SpriteRenderer spriteRenderer;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite destroyedSprite;
    public GameObject deathVFX;
    private GameObject explosionHandler;

    public float tempHealth;
    float attackCDLeft;
    private Collider2D entityCollider;

    private PlayerHandler inputHandler;
    private ObjectFadeEffect objectFader;

    [SerializeField] private Transform pfBullet;
    private Transform bulletSpawn;

    //Coin variables
    [SerializeField] private GameObject pfCoin;
    bool hasSpawned;
    private bool isExploding = false;

    private AIBase aStar;
    //NavMeshProperties


    void Start()
    {
        // Initialize to the initial state (e.g., Idle)
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        tempHealth = enemyData.health;
        currentState = EnemyState.move;
        bulletSpawn = GetComponentInChildren<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        objectFader = GetComponent<ObjectFadeEffect>();

        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        aStar = GetComponent<AIBase>();

    }

    public void TakeDamage(float damage)
    {
        tempHealth -= damage;
        if (tempHealth <= 0)
        {
            currentState = EnemyState.death;
        }
    }

    void CheckState(float dis)
    {
        if (currentState != EnemyState.death)
        {
            if (dis <= enemyData.attackRange)
            {
                // Attack the player
                currentState = EnemyState.attack;
            }
            else
            {
                // Move towards the player
                currentState = EnemyState.move;
            }
        }

        else return;
    }

    void MovetoPlayer()
    {
        //Vector3 direction = (player.transform.position - transform.position).normalized;
        //transform.Translate(direction * enemyData.speed * Time.deltaTime);
        //agent.SetDestination(player.position);

    }

    void Attack()
    {
        attackCDLeft -= Time.deltaTime;
        if(attackCDLeft <= 0f)
        {
            playerLastLoc = player.transform;
            attackCDLeft = enemyData.attackSpeed;
            Transform bulletTransform = Instantiate(pfBullet, bulletSpawn.position, Quaternion.identity);
            Vector3 shootDir = (playerLastLoc.position - bulletSpawn.position).normalized;
            if(bulletTransform.GetComponent<Bullet>() != null)
            {
                bulletTransform.GetComponent<Bullet>().SetUp(shootDir);
            }

            else
            {
                return;
            }
        }
    }

    public void Death()
    {
        aStar.canMove = false;
        entityCollider.enabled = false;
        spriteRenderer.sprite = destroyedSprite;
        if (isExploding != true)
        {
            SpawnExplosion();
        }
        objectFader.StartFading();
    }

    void SpawnExplosion()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        explosionHandler = explosion;
        isExploding = true;
    }

    void DropLoot()
    {
        if (!hasSpawned)
        {
            SpawnCoin();
            inputHandler.ChargeUltimate(5);
            hasSpawned = true;
        }
    }

    public void IntroDeath()
    {
        entityCollider.enabled = false;
        spriteRenderer.sprite = destroyedSprite;
        Destroy(gameObject, 3f);
    }

    private void SpawnCoin()
    {
        //Spawn GNA
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        GameObject coin = Instantiate(pfCoin, transform.position, Quaternion.identity);
        coin.transform.Rotate(0, 0, 90);
    }

    void CheckFlip()
    {
        Vector3 direction = player.position - transform.position;

        // Check the direction and set the sprite accordingly.
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {

            spriteRenderer.sprite = leftSprite;

            if (direction.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                spriteRenderer.sprite = upSprite;
            }
            else
            {
                spriteRenderer.sprite = downSprite;
            }
        }

    }

    private void Update()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        CheckState(distanceToPlayer);

        switch (currentState)
        {
            case EnemyState.death:
                DropLoot();
                Death();
                break;

            case EnemyState.attack:
                CheckFlip();
                Attack();
                break;

            case EnemyState.move:
                CheckFlip();
                MovetoPlayer();
                break;

        }

    }
}
