using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : MonoBehaviour, IActivatable
{
    public enum EnemyState
    {
        idle,
        roam,
        chase,
        attack,
        death,
    }

    //Public variables
    public EnemyState entityState;
    public EnemyScriptableObject enemyData;
    public float changeDirectionInterval;
    public float maxWanderDistance;

    //Private variables
    private PlayerHealthScript playerHealth;
    private PlayerHandler inputHandler;
    private LevelManager levelManager;
    private Animator anim;
    private Transform playerTransform;
    private Collider2D entityCollider;
    private Vector2 targetPosition;
    private float timeSinceLastDirectionChange;
    private bool isTriggered;
    private bool hasSpawned;
    private bool hasDied;
    private bool isKicking;

    //Serializable variables
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempDamage;
    [SerializeField] float tempAtkSpd;
    [SerializeField] float tempRange;
    [SerializeField] float attackDamageCooldown;
    [SerializeField] bool isBlocked;
    [SerializeField] Transform blockingEntity;
    [SerializeField] ScoreManagerScript scoreManager;
    [SerializeField] AudioManagerScript audioManager;

    // Start is called before the first frame update
    void Start()
    {
        //External Checks
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthScript>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManagerScript>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManger").GetComponent<AudioManagerScript>();

        //Internal Checks
        anim = GetComponent<Animator>();
        entityCollider = GetComponent<Collider2D>();

        //Set Variables
        AssignStat();
        this.enabled = false;
    }
    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempDamage = enemyData.attackDamage;
        tempAtkSpd = enemyData.attackSpeed;
        tempRange = enemyData.attackRange;
        attackDamageCooldown = tempSpeed;
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

    public void Activate()
    {
        this.enabled = true;
    }

    void Idle()
    {
        anim.SetBool("isIdle", true);
    }

    void Roam()
    {
        anim.SetBool("isMove", true);
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
            entityState = EnemyState.chase;
        }
    }

    void Chase()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if(distanceToPlayer < tempRange)
        {
            entityState = EnemyState.attack;
        }

        else
        {
            // Calculate the direction towards the player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Move towards the player
            transform.Translate(direction * tempSpeed * Time.deltaTime);
        }
    }

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
            entityState = EnemyState.chase;
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
            audioManager.PlaycivillianDeathSFX();
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
        switch (entityState)
        {
            case EnemyState.idle:
                Idle();
                break;

            case EnemyState.roam:
                Roam();
                break;

            case EnemyState.chase:
                Chase();
                break;

            case EnemyState.attack:
                Attack();
                break;

            case EnemyState.death:
                Death();
                break;
        }
    }
}
