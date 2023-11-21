using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    public enum EnemyState { fall, run, death, walk, }

    public EnemyState enemyState;
    SpriteRenderer spriteRenderer;
    public EnemyScriptableObject enemyData;
    Rigidbody2D rb;
    public float detectionDistance = 4f;

    public Collider2D entityCollider;

    public Transform player;
    public Animator anim;
    public GameObject deathVFX;
    public ObjectFadeEffect fadeEffect;

    private float lastPosX;
    private float runSpeed;

    [SerializeField] private LevelManager levelManager;
   

    bool isTriggered;
    bool hasDied;

    //Wandering Variables
    public float changeDirectionInterval = 1.0f; // Time interval to change direction
    public float maxWanderDistance = 8.0f; // Maximum distance to wander from the starting point

    private Vector2 targetPosition;
    private float timeSinceLastDirectionChange = 0.0f;
    private PlayerHandler inputHandler;
    private Transform blockingEntity;
    private EventManager eventManager;

    public bool isBlocked;
    private bool hasSpawned;

    public FakeHeightScript fakeheight;

    public AudioSource civilianAudioSource;
    public AudioClip[] DeathSFX;
    private bool deathSFXPlayed = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        entityCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();

        fadeEffect = GetComponent<ObjectFadeEffect>();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        lastPosX = transform.position.x;

        targetPosition = transform.position;

        RandomizeSpeed(enemyData.speed);
    }

    private void RandomizeSpeed(float speed)
    {
        runSpeed = Random.Range(enemyData.speed, enemyData.speed + 2f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerLeg"))
        {
            inputHandler.ChargeUltimate(1);
            enemyState = EnemyState.death;
            Debug.Log("Hit");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BigBuilding")
        {
            blockingEntity = collision.gameObject.transform;
            isBlocked = true;
        }
    }


    void Run(Vector2 dir)
    {
        if(enemyState != EnemyState.death)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float newDistance = detectionDistance + 5f;

            if (distanceToPlayer < newDistance)
            {
                if (isBlocked != true)
                {
                    // Calculate the direction away from the player
                    Vector3 runDirection = transform.position - player.position;

                    // Normalize the direction to get a unit vector
                    runDirection.Normalize();

                    // Move the enemy in the runDirection
                    transform.position += runDirection * runSpeed * Time.deltaTime;
                }

                else
                {
                    Vector3 newRunDirection = transform.position - blockingEntity.transform.position;

                    newRunDirection.Normalize();
                    // Calculate a new target position within the wander range
                    transform.position += newRunDirection * runSpeed * Time.deltaTime;
                }
            }

            else if (distanceToPlayer > newDistance)
            {
                ChangeWalkState();
            }
        }
    }

    void Walk()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within the detection distance
        if (distanceToPlayer < detectionDistance)
        {
            anim.SetBool("run", true);
            anim.SetBool("walk", false);
            enemyState = EnemyState.run;
        }

        else
        {
            anim.SetBool("walk", true);
            anim.SetBool("run", false);
            timeSinceLastDirectionChange += Time.deltaTime;

            // Check if it's time to change direction
            if (timeSinceLastDirectionChange >= changeDirectionInterval)
            {
                if (isBlocked != true)
                {
                    // Generate a random direction vector
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;

                    // Calculate a new target position within the wander range
                    targetPosition = (Vector2)transform.position + randomDirection * Random.Range(0.0f, maxWanderDistance);
                }

                else
                {
                    Vector3 newRunDirection = transform.position - blockingEntity.transform.position;

                    newRunDirection.Normalize();
                    // Calculate a new target position within the wander range
                    transform.position += newRunDirection * enemyData.speed * Time.deltaTime;

                    isBlocked = false;
                }

                // Reset the timer
                timeSinceLastDirectionChange = 0.0f;
            }

            // Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemyData.speed * Time.deltaTime);
        }
    }

    void ChangeRunState()
    {
        anim.SetBool("run", true);
        anim.SetBool("walk", false);
        enemyState = EnemyState.run;
        fakeheight.isGrounded = true;
    }

    void ChangeWalkState()
    {
        anim.SetBool("walk", true);
        anim.SetBool("run", false);
        enemyState = EnemyState.walk;
    }

    void FallToRun()
    {
        if(fakeheight.isGrounded == true)
        {
            PlayDeathSFX();
            entityCollider.enabled = true;
            enemyState = EnemyState.death;
        }
        
    }

    public void Death()
    {
       
        if (!isTriggered)
        {
            levelManager.CalculateScore(0.1f);
            isTriggered = true;
        }
        entityCollider.enabled = false;

        if (!hasSpawned)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
            hasSpawned = true;
        }

        if (!hasDied)
        {
            eventManager.AddScore();
            hasDied = true;
        }

        // Check if the death sound effect has already been played
        if (!deathSFXPlayed)
        {
            PlayDeathSFX();
            deathSFXPlayed = true; // Set the flag to true to indicate that the sound effect has been played
        }

        Destroy(transform.parent.gameObject, 5f);
    }

    void FlipSprite()
    {
        // Check if the sprite's horizontal position has changed since the last frame
        float currentPositionX = transform.position.x;
        if (currentPositionX > lastPosX)
        {
            // Moving right, so flip the sprite to face right
            spriteRenderer.flipX = true;
        }
        else if (currentPositionX < lastPosX)
        {
            // Moving left, so flip the sprite to face left
            spriteRenderer.flipX = false;
        }

        // Update the last position for the next frame
        lastPosX = currentPositionX;
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();

        switch (enemyState)
        {
            case EnemyState.death:
                anim.SetBool("death", true);
                spriteRenderer.sortingOrder = 2;
                Death();
                break;

            case EnemyState.fall:
                anim.SetBool("fall", true);
                spriteRenderer.sortingOrder = 4;
                FallToRun();
                break;

            case EnemyState.run:
                Run(player.position);
                break;

            case EnemyState.walk:
                Walk();
                break;

        }
    }

    public void PlayDeathSFX()
    {
        AudioClip deathsoundtoPlay = DeathSFX[Random.Range(0, DeathSFX.Length)];
        civilianAudioSource.PlayOneShot(deathsoundtoPlay);
    }
}
