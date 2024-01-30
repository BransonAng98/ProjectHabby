using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCow : MonoBehaviour
{
    public enum CowState
    {
        idle,
        roam,
        panic,
        detonate,
        death,
    }

    //Public Variables
    public CowState entityState;
    public EnemyScriptableObject enemyData;
    public List<GameObject> followerCowList;
    public float changeDirectionInterval;
    public float maxWanderDistance;
    public Transform blockingEntity;

    //Private Variables
    private PlayerHandler inputHandler;
    private PlayerHealthScript playerHealth;
    private ScoreManagerScript scoreManager;
    private AudioManagerScript audioManager;
    private Collider2D entityCollider;
    private Animator anim;
    private float timeSinceLastDirectionChange;
    private Transform playerTransform;
    private Vector2 targetPosition;
    private Vector2 randomDirection;

    //Serializable Variables
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempDamage;
    [SerializeField] float tempRange;
    [SerializeField] float explosionRadius;
    [SerializeField] bool isBlocked;
    [SerializeField] bool isDetonating;
    [SerializeField] bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthScript>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManagerScript>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManger").GetComponent<AudioManagerScript>();

        //Internal Check
        entityCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        //Set Variables
        AssignStat();
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempDamage = enemyData.attackDamage;
        tempRange = enemyData.attackRange;
    }

    // Call this function when spawning the cows to assign the follower cows to the leader cow
    public void AssignToList(GameObject cow)
    {
        followerCowList.Add(cow);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDetonating)
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

        else
        {
            entityState = CowState.detonate;
        }
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
            entityState = CowState.panic;

            if (!isTriggered)
            {
                ChooseRandomDirection();
            }
        }
    }
    void ChooseRandomDirection()
    {
        // Choose a random direction
        float randomAngle = Random.Range(0f, 360f);
        randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        isTriggered = true;
    }

    void Panic()
    {
        transform.Translate(randomDirection * tempSpeed * Time.deltaTime);
    }

    void Detonate()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            Targetable affectedEntity = collider.GetComponent<Targetable>();
            if(affectedEntity != null)
            {
                affectedEntity.TakeDamage(tempDamage);
                entityState = CowState.death;
            }
        }
    }

    void Death()
    {
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
            case CowState.idle:
                Idle();
                break;

            case CowState.roam:
                Roam();
                break;

            case CowState.panic:
                Panic();
                break;

            case CowState.detonate:
                Detonate();
                break;

            case CowState.death:
                Death();
                break;
        }
    }
}
