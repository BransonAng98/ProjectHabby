using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public enum soliderState
    {
        idle,
        move,
        attack,
        run,
        death,
    }

    //Public variables
    public EnemyScriptableObject enemyData;
    public GameObject targetBuilding;
    public Transform[] waypoints;
    public float maxWanderDistance;
    public float changeDirectionInterval;
    public Transform blockingEntity;
    public LevelManager levelManager;
    public ScoreManagerScript scoreManager;
    public AudioManagerScript audiomanager;

    //Serialized variables
    [SerializeField] PlayerHealthScript playerHealth;
    [SerializeField] PlayerHandler inputHandler;
    [SerializeField] Transform playerTransform;
    [SerializeField] Animator anim;
    [SerializeField] float attackDamageCooldown;

    [SerializeField] bool isBlocked;
    [SerializeField] bool hasSpawned;
    [SerializeField] bool hasDied;
    [SerializeField] bool deathSFXPlayed;
    [SerializeField] bool isKicking;

    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempDamage;
    [SerializeField] float tempAtkSpd;
    [SerializeField] float tempRange;

    //private variables
    private soliderState entityState;
    private bool isFacingRight = false;
    private int currentWaypointIndex = 0;
    private Vector2 targetPosition;
    private float timeSinceLastDirectionChange;
    private bool isTriggered;
    private Collider2D entityCollider;
    // Start is called before the first frame update
    void Start()
    {
        //Internal Checks
        anim = GetComponent<Animator>();
        entityCollider = GetComponent<Collider2D>();

        //External Checks
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthScript>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManagerScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();

        //Setting Variables
        AssignStat();
        attackDamageCooldown = tempSpeed;
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempDamage = enemyData.attackDamage;
        tempAtkSpd = enemyData.attackSpeed;
        tempRange = enemyData.attackRange;
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
    }

    //Idling 
    void Idle()
    {
        anim.SetBool("isIdle", true);
    }

    void PatOrMove()
    {
        anim.SetBool("isMove", true);
        if (targetBuilding != null)
        {
            Patrol();
        }

        else
        {
            Move();
        }
    }

    //Moving with an assigned building
    void Patrol()
    {
        if (!PlayerInRange())
        {
            if (waypoints.Length == 0)
            {
                Debug.LogError("No waypoints assigned to the enemy!");
                return;
            }

            Transform targetWaypoint = waypoints[currentWaypointIndex];

            // Move towards the current waypoint
            transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, tempSpeed * Time.deltaTime);

            // Check if the enemy has reached the waypoint
            if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                // Switch to the next waypoint
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }

            // Flip the sprite based on the direction of movement
            if (transform.position.x < targetWaypoint.position.x)
            {
                // Moving right
                FlipSprite(false);
            }
            else
            {
                // Moving left
                FlipSprite(true);
            }
        }

        else
        {
            //Enter attack state when player is within attacking range
            entityState = soliderState.attack;
        }
    }

    //Moving without an assigned building
    void Move()
    {
        if (!PlayerInRange())
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
                    if (blockingEntity != null)
                    {
                        Vector3 newRunDirection = transform.position - blockingEntity.transform.position;

                        newRunDirection.Normalize();
                        // Calculate a new target position within the wander range
                        transform.position += newRunDirection * tempSpeed * Time.deltaTime;

                        isBlocked = false;
                    }

                    else
                    {
                        isBlocked = false;
                    }
                }
            }

            // Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, tempSpeed * Time.deltaTime);
        }
        else
        {
            entityState = soliderState.run;
        }
    }

    //Attacking the player
    void Attack()
    {
        if (PlayerInRange())
        {
            anim.SetBool("isAttack", true);

            //Stop the soldier from moving 
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            if (attackDamageCooldown != 0)
            {
                attackDamageCooldown -= 1f * Time.deltaTime;
            }

            else
            {
                attackDamageCooldown = tempAtkSpd;
                playerHealth.TakeDamage(tempDamage);
            }
        }

        else
        {
            //reverts back to moving state when player is out of attack range
            entityState = soliderState.move;
        }
    }

    //Running away from the player
    void Run()
    {
        if (PlayerInRange())
        {
            anim.SetBool("isRun", true);
            if (!isBlocked)
            {
                // Calculate the direction away from the player
                Vector3 runDirection = transform.position - playerTransform.position;

                // Normalize the direction to get a unit vector
                runDirection.Normalize();

                // Move the enemy in the runDirection
                transform.position += runDirection * tempSpeed * 1.5f * Time.deltaTime;
            }

            else
            {
                Vector3 newRunDirection = transform.position - blockingEntity.transform.position;

                newRunDirection.Normalize();
                // Calculate a new target position within the wander range
                transform.position += newRunDirection * tempSpeed * 1.5f * Time.deltaTime;
            }
        }

        else
        {
            entityState = soliderState.move;
        }
    }

    void Death()
    {
        if (!isTriggered)
        {
            levelManager.CalculateScore(1f);
            inputHandler.ChargeUltimate(1);
            isTriggered = true;
        }
        entityCollider.enabled = false;

        if (!hasSpawned)
        {
            ObjectPooler.Instance.SpawnFromPool("BloodSplatter", transform.position, Quaternion.identity);
            hasSpawned = true;
        }

        if (!hasDied)
        {
            hasDied = true;
        }

        // Check if the death sound effect has already been played
        if (!deathSFXPlayed)
        {
            audiomanager.PlaycivillianDeathSFX();
            //PlayDeathSFX();
            deathSFXPlayed = true; // Set the flag to true to indicate that the sound effect has been played
        }

        if (isKicking == false)
        {
            scoreManager.amtOfcivilians += 1;
            scoreManager.goldearned += 1;
        }

        GameObject deadbody = ObjectPooler.Instance.SpawnFromPool("DeadCivi", transform.position, Quaternion.identity);
        deadbody.GetComponent<ObjectFadeEffect>().StartFading();

        Destroy(transform.parent.gameObject);
    }

    void FlipSprite(bool flipLeft)
    {
        // Flip the sprite based on the specified direction
        Vector3 scale = transform.localScale;
        scale.x = flipLeft ? -1f : 1f;
        transform.localScale = scale;
    }

    bool PlayerInRange()
    {
        if (playerTransform != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Check if the player is within the detection range
            return distanceToPlayer <= tempRange;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch(entityState)
        {
            case soliderState.idle:
                Idle();
                break;

            case soliderState.move:
                PatOrMove();
                break;

            case soliderState.attack:
                Attack();
                break;

            case soliderState.run:
                Run();
                break;

            case soliderState.death:
                Death();
                break;
        }
    }
}
