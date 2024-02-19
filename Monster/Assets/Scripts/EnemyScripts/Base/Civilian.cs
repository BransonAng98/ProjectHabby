using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    public enum EnemyState { fall, run, death, walk, idle}

    public EnemyState enemyState;
    SpriteRenderer spriteRenderer;
    public EnemyScriptableObject enemyData;
    public float detectionDistance = 4f;

    public Collider2D entityCollider;

    public Transform player;
    public Animator anim;
   
    private float lastPosX;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    //[SerializeField] private LevelManager levelManager;

    //Idle Time Variable
    [SerializeField] float idleTime;
    [SerializeField] float currentIdleTime;

    bool isTriggered;
    bool hasDied;

    //Follower Variables
    [SerializeField] Transform leaderPos;
    [SerializeField] bool isFollowing;

    //Wandering Variables
    public float changeDirectionInterval = 1.0f; // Time interval to change direction
    public float maxWanderDistance = 8.0f; // Maximum distance to wander from the starting point

    private Vector2 targetPosition;
    private float timeSinceLastDirectionChange = 0.0f;
    //private PlayerHandler inputHandler;
    [SerializeField] private Transform blockingEntity;
    //private EventManager eventManager;
    //private GameManagerScript gamemanager;

    public bool isBlocked;
    private bool hasSpawned;

    public FakeHeightScript fakeheight;
    //public float rotationSpeed;
    public float minRotationSpeed;
    public float maxRotationSpeed;

    public AudioSource civilianAudioSource;
    public AudioClip[] DeathSFX;
    //public AudioManagerScript audiomanager;
    private bool deathSFXPlayed = false;
    public GameObject deadSprite;

    //Troubleshoot
    public string causeOfDeath;
    public string murderer;
    public KickHeightScript fakekickheight;
    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;
    public bool isKicking;
    public float rotationSpeed;

    //public ScoreManagerScript scoremanager;
    private void Awake()
    {
        //scoremanager = GameObject.Find("ScoreManager").GetComponent<ScoreManagerScript>();
        fakeheight = GetComponentInParent<FakeHeightScript>();
        anim = GetComponent<Animator>();
        entityCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        //eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        //levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        //gamemanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        //audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        SetValue();
    }


    // Start is called before the first frame update
    void Start()
    {
        lastPosX = transform.position.x;

        targetPosition = transform.position;

        RandomizeSpeed(enemyData.speed);
        RandomizeIdle();
    }

    void SetValue()
    {
        int setSpinValue = Random.Range(500, 700);
        {
            rotationSpeed = setSpinValue;
        }
    }


    void RandomizeIdle()
    {
        idleTime = Random.Range(1, 3);
    }

    private void RandomizeSpeed(float speed)
    {
        walkSpeed = Random.Range(enemyData.speed - 0.2f, enemyData.speed);
        runSpeed = Random.Range(enemyData.speed + 1f, enemyData.speed + 1.5f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerLeg"))
        {
            //inputHandler.ChargeUltimate(1);
            causeOfDeath = "Stepped to death";
            murderer = collision.name;
            enemyState = EnemyState.death;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BigBuilding")
        {
            blockingEntity = collision.gameObject.transform;
            isBlocked = true;
        }
        if (collision.gameObject.tag == "Blocker")
        {
            blockingEntity = collision.gameObject.transform;
            isBlocked = true;
        }


        if (collision.gameObject.tag == "Player")
        {
            causeOfDeath = "Stepped to death";
            enemyState = EnemyState.death;
        }
    }

    public void AddCivilian(Transform leaderGO)
    {
        leaderPos = leaderGO;
        isFollowing = true;
    }

    public void RemoveCivilan()
    {
        isFollowing = false;
        leaderPos = null;
    }

    void Run()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float newDistance = detectionDistance + 5f;

        if (distanceToPlayer < newDistance)
        {
            if (!isBlocked)
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
            enemyState = EnemyState.walk;
            anim.SetBool("walk", true);
        }
    }
       

    void IdleOrMove()
    {
        float decision = Random.Range(0, 1 + 1);
        switch (decision)
        {
            case 0:
                enemyState = EnemyState.idle;
                anim.SetBool("idle", true);
                anim.SetBool("walk", false);
                break;

            case 1:
                timeSinceLastDirectionChange = 0.0f;
                enemyState = EnemyState.walk;
                anim.SetBool("walk", true);
                anim.SetBool("idle", false);
                break;
        }
    }

    void Idle()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionDistance)
        {
            anim.SetBool("idle", false);
            anim.SetBool("run", true);
            anim.SetBool("walk", false);
            enemyState = EnemyState.run;
        }

        else
        {
            anim.SetBool("idle", true);
            anim.SetBool("run", false);
            anim.SetBool("walk", false);

            if (currentIdleTime <= idleTime)
            {
                currentIdleTime += Time.deltaTime;
            }

            else
            {
                IdleOrMove();
                currentIdleTime = 0f;
            }
        }
      
    }

    void Walk()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within the detection distance
        if (distanceToPlayer < detectionDistance)
        {
            anim.SetBool("idle", false);
            anim.SetBool("run", true);
            anim.SetBool("walk", false);
            enemyState = EnemyState.run;
        }

        else
        {
            anim.SetBool("idle", false);
            anim.SetBool("walk", true);
            anim.SetBool("run", false);

            if (!isFollowing)
            {
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
                        if(blockingEntity != null)
                        {
                            Vector3 newRunDirection = transform.position - blockingEntity.transform.position;

                            newRunDirection.Normalize();
                            // Calculate a new target position within the wander range
                            transform.position += newRunDirection * walkSpeed * Time.deltaTime;

                            isBlocked = false;
                        }

                        else
                        {
                            isBlocked = false;
                        }
                    }

                    // Reset the timer
                    IdleOrMove();
                }

                // Move towards the target position
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
            }
            else
            {
                //MoveTowards(leaderPos.position, walkSpeed);
            }
        }
    }

    void MoveTowards(Vector2 targetPosition, float speed)
    {
        if(leaderPos != null)
        {
            // Calculate the direction to the target
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Move towards the target using Translate
            transform.Translate(direction * speed * Time.deltaTime);
        }

        else
        {
            Debug.LogError("Leader is dead, but I am still following orders");
        }
    }

    void FallToRun()
    {
        // Randomize rotationSpeed within a specified range
        float randomRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        transform.Rotate(Vector3.forward, randomRotationSpeed * Time.deltaTime);  // spin him around

        if (fakeheight.isGrounded == true)
        {
            //audiomanager.PlaycivillianDeathSFX();
            //entityCollider.enabled = true;
            enemyState = EnemyState.death;
        }
    }
    public void Death()
    {
        if (!isTriggered)
        {
            //levelManager.CalculateScore(1f);
            //inputHandler.ChargeUltimate(1);
            isTriggered = true;
        }
        entityCollider.enabled = false;

        if (!hasSpawned)
        {
            ObjectPooler.Instance.SpawnFromPool("BloodSplatter",transform.position, Quaternion.identity);
            hasSpawned = true;
        }

        if (!hasDied)
        {
            hasDied = true;
        }

        // Check if the death sound effect has already been played
        if (!deathSFXPlayed)
        {
            //audiomanager.PlaycivillianDeathSFX();
            //PlayDeathSFX();
            deathSFXPlayed = true; // Set the flag to true to indicate that the sound effect has been played
        }

        if(isKicking == false)
        {
            //scoremanager.amtOfcivilians += 1;
            //scoremanager.goldearned += 1;
        }

        GameObject deadbody = ObjectPooler.Instance.SpawnFromPool("DeadCivi", transform.position, Quaternion.identity);
        deadbody.GetComponent<ObjectFadeEffect>().StartFading();
        deadbody.GetComponent<CauseOfDeath>().causeOfDeath = causeOfDeath;
        deadbody.GetComponent<CauseOfDeath>().whoKilledMe = murderer;

        Destroy(transform.parent.gameObject);
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

    void StopMovement()
    {
        walkSpeed = 0f;
        runSpeed = 0f;
    }
    // Update is called once per frame
    void Update()
    {
      
        FlipSprite();

        switch (enemyState)
        {
            case EnemyState.death:
                anim.SetBool("death", true);
                spriteRenderer.sortingOrder = 3;
                Death();
                break;

            case EnemyState.fall:
                anim.SetBool("fall", true);
                isKicking = true;
                spriteRenderer.sortingOrder = 4;
                FallToRun();
                break;

            case EnemyState.run:
                Run();
                break;

            case EnemyState.walk:
                Walk();
                break;

            case EnemyState.idle:
                Idle();
                break;

        }

        //if(gamemanager.gameEnded== true)
        //{
        //    StopMovement();
        //}
    }

    /*public void PlayDeathSFX()
    {
        AudioClip deathsoundtoPlay = DeathSFX[Random.Range(0, DeathSFX.Length)];
        civilianAudioSource.PlayOneShot(deathsoundtoPlay);
    }*/
}
